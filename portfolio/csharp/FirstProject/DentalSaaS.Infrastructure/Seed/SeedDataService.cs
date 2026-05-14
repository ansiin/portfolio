using DentalSaaS.Domain.Entities;
using DentalSaaS.Infrastructure.Persistence;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DentalSaaS.Infrastructure.Seed;

public static class SeedDataService
{
    public static Task ForceReseedAcmeAsync(IServiceProvider services, CancellationToken ct = default)
        => SeedAsync(services, forceDemoDataReset: true, ct);

    public static async Task SeedAsync(IServiceProvider services, bool forceDemoDataReset = false, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<AppUserRole>>();

        await InitializeDatabaseAsync(db, ct);

        await SeedRolesAsync(roles);
        await SeedSystemUsersAsync(users);

        var company = await db.Companies.SingleOrDefaultAsync(c => c.Slug == "acme", ct);
        if (company is null)
        {
            company = new Company
            {
                Name = "Acme Dental",
                Slug = "acme",
                Tier = SubscriptionTier.Free,
                Settings = new CompanySettings
                {
                    CountryCode = "US",
                    XrayIntervalDays = 180,
                    CreatedBy = "system"
                }
            };

            company.Settings.CompanyId = company.Id;
            await db.Companies.AddAsync(company, ct);
            await db.CompanySettings.AddAsync(company.Settings, ct);
            await db.SaveChangesAsync(ct);
        }

        if (forceDemoDataReset)
        {
            await PurgeCompanyDataAsync(db, company.Id, ct);
        }

        var hasSettings = await db.CompanySettings
            .IgnoreQueryFilters()
            .AnyAsync(s => s.CompanyId == company.Id, ct);
        if (!hasSettings)
        {
            await db.CompanySettings.AddAsync(new CompanySettings
            {
                CompanyId = company.Id,
                CountryCode = "US",
                XrayIntervalDays = 180,
                CreatedBy = "system"
            }, ct);
        }

        var owner = await EnsureTenantUserAsync(users, "owner@acme.local", "Acme Owner", "Owner123!");
        var admin = await EnsureTenantUserAsync(users, "admin@acme.local", "Acme Admin", "Admin123!");
        var manager = await EnsureTenantUserAsync(users, "manager@acme.local", "Acme Manager", "Manager123!");
        var employee = await EnsureTenantUserAsync(users, "employee@acme.local", "Acme Employee", "Employee123!");

        var memberships = new[]
        {
            new CompanyMembership { CompanyId = company.Id, UserId = owner.Id.ToString(), Role = Roles.CompanyOwner },
            new CompanyMembership { CompanyId = company.Id, UserId = admin.Id.ToString(), Role = Roles.CompanyAdmin },
            new CompanyMembership { CompanyId = company.Id, UserId = manager.Id.ToString(), Role = Roles.CompanyManager },
            new CompanyMembership { CompanyId = company.Id, UserId = employee.Id.ToString(), Role = Roles.CompanyEmployee }
        };

        foreach (var membership in memberships)
        {
            var exists = await db.CompanyMemberships
                .AnyAsync(m => m.CompanyId == membership.CompanyId && m.UserId == membership.UserId, ct);
            if (!exists)
            {
                await db.CompanyMemberships.AddAsync(membership, ct);
            }
        }

        await EnsureAcmePremiumSubscriptionAsync(db, company, ct);
        await SeedDemoOperationalDataAsync(db, company, manager.Id.ToString(), ct);

        await db.SaveChangesAsync(ct);
    }

    private static async Task NormalizeLegacyDatabaseStateAsync(AppDbContext db, CancellationToken ct)
    {
        if (!IsSqliteProvider(db))
        {
            return;
        }

        var connection = db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
        {
            await connection.OpenAsync(ct);
        }

        try
        {
            var hasHistory = await TableExistsAsync(connection, "__EFMigrationsHistory", ct);
            var appliedMigrations = hasHistory
                ? await GetHistoryRowCountAsync(connection, ct)
                : 0;

            if (appliedMigrations > 0)
            {
                return;
            }

            var hasLegacyTables =
                await TableExistsAsync(connection, "Companies", ct) ||
                await TableExistsAsync(connection, "Patients", ct) ||
                await TableExistsAsync(connection, "AuditLogs", ct) ||
                await TableExistsAsync(connection, "AspNetUsers", ct);

            if (hasLegacyTables)
            {
                await connection.CloseAsync();
                await db.Database.EnsureDeletedAsync(ct);
            }
        }
        finally
        {
            if (shouldClose && connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static async Task<bool> TableExistsAsync(System.Data.Common.DbConnection connection, string tableName, CancellationToken ct)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = $name;";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "$name";
        parameter.Value = tableName;
        command.Parameters.Add(parameter);

        var result = await command.ExecuteScalarAsync(ct);
        var count = Convert.ToInt64(result ?? 0);
        return count > 0;
    }

    private static async Task<long> GetHistoryRowCountAsync(System.Data.Common.DbConnection connection, CancellationToken ct)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM \"__EFMigrationsHistory\";";
        var result = await command.ExecuteScalarAsync(ct);
        return Convert.ToInt64(result ?? 0);
    }

    private static async Task ReleaseStaleSqliteMigrationLockAsync(AppDbContext db, CancellationToken ct)
    {
        if (!IsSqliteProvider(db))
        {
            return;
        }

        var connection = db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
        {
            await connection.OpenAsync(ct);
        }

        try
        {
            var hasLockTable = await TableExistsAsync(connection, "__EFMigrationsLock", ct);
            if (!hasLockTable)
            {
                return;
            }

            await db.Database.ExecuteSqlRawAsync("DELETE FROM \"__EFMigrationsLock\" WHERE \"Id\" = 1;", ct);
        }
        finally
        {
            if (shouldClose && connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static async Task InitializeDatabaseAsync(AppDbContext db, CancellationToken ct)
    {
        const int maxAttempts = 12;
        var delay = TimeSpan.FromSeconds(5);
        Exception? lastException = null;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await NormalizeLegacyDatabaseStateAsync(db, ct);
                await ReleaseStaleSqliteMigrationLockAsync(db, ct);
                await db.Database.MigrateAsync(ct);
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                lastException = ex;
                await Task.Delay(delay, ct);
            }
        }

        throw new InvalidOperationException(
            $"Failed to initialize the database after {maxAttempts} attempts.",
            lastException);
    }

    private static bool IsSqliteProvider(AppDbContext db)
        => string.Equals(
            db.Database.ProviderName,
            "Microsoft.EntityFrameworkCore.Sqlite",
            StringComparison.Ordinal);

    private static async Task SeedRolesAsync(RoleManager<AppUserRole> roles)
    {
        foreach (var role in SeedCatalog.SeedRoles)
        {
            if (!await roles.RoleExistsAsync(role))
            {
                await roles.CreateAsync(new AppUserRole { Name = role });
            }
        }
    }

    private static async Task SeedSystemUsersAsync(UserManager<AppUser> users)
    {
        var admin = await EnsureTenantUserAsync(users, "systemadmin@dentalsaas.local", "System Admin", "SystemAdmin123!");
        var support = await EnsureTenantUserAsync(users, "support@dentalsaas.local", "System Support", "SystemSupport123!");
        var billing = await EnsureTenantUserAsync(users, "billing@dentalsaas.local", "System Billing", "SystemBilling123!");

        await EnsureGlobalRoleAsync(users, admin, Roles.SystemAdmin);
        await EnsureGlobalRoleAsync(users, support, Roles.SystemSupport);
        await EnsureGlobalRoleAsync(users, billing, Roles.SystemBilling);
    }

    private static async Task<AppUser> EnsureTenantUserAsync(UserManager<AppUser> users, string email, string displayName, string password)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = await users.FindByEmailAsync(normalized);
        if (user is not null)
        {
            return user;
        }

        user = new AppUser
        {
            UserName = normalized,
            Email = normalized,
            DisplayName = displayName
        };

        var created = await users.CreateAsync(user, password);
        if (!created.Succeeded)
        {
            throw new InvalidOperationException($"Failed to seed user '{email}': {string.Join("; ", created.Errors.Select(e => e.Description))}");
        }

        return user;
    }

    private static async Task EnsureGlobalRoleAsync(UserManager<AppUser> users, AppUser user, string role)
    {
        if (!await users.IsInRoleAsync(user, role))
        {
            await users.AddToRoleAsync(user, role);
        }
    }

    private static async Task PurgeCompanyDataAsync(AppDbContext db, Guid companyId, CancellationToken ct)
    {
        await db.PaymentPlans
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.PlanItems
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.Appointments
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.ToothRecords
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.Xrays
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.Treatments
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.CostEstimates
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.Invoices
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.TreatmentPlans
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.InsurancePlans
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.TreatmentTypes
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.TreatmentRooms
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.Dentists
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.Patients
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.AuditLogs
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.Subscriptions
            .IgnoreQueryFilters()
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);

        await db.ImpersonationSessions
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);
    }

    private static async Task EnsureAcmePremiumSubscriptionAsync(AppDbContext db, Company company, CancellationToken ct)
    {
        var subscriptions = await db.Subscriptions
            .Where(s => s.CompanyId == company.Id)
            .ToListAsync(ct);

        var active = subscriptions
            .OrderByDescending(s => s.ValidFrom)
            .FirstOrDefault();

        if (active is null)
        {
            await db.Subscriptions.AddAsync(new Subscription
            {
                CompanyId = company.Id,
                Tier = SubscriptionTier.Premium,
                CreatedBy = "system"
            }, ct);
            company.Tier = SubscriptionTier.Premium;
            db.Companies.Update(company);
            return;
        }

        if (active.Tier == SubscriptionTier.Premium)
        {
            company.Tier = SubscriptionTier.Premium;
            db.Companies.Update(company);
            return;
        }

        active.IsDeleted = true;
        active.ValidTo = DateTimeOffset.UtcNow;
        active.DeletedAt = DateTimeOffset.UtcNow;
        active.DeletedBy = "system";
        db.Subscriptions.Update(active);

        await db.Subscriptions.AddAsync(new Subscription
        {
            CompanyId = company.Id,
            Tier = SubscriptionTier.Premium,
            CreatedBy = "system"
        }, ct);

        company.Tier = SubscriptionTier.Premium;
        db.Companies.Update(company);
    }

    private static async Task SeedDemoOperationalDataAsync(AppDbContext db, Company company, string createdBy, CancellationToken ct)
    {
        var hasPatients = await db.Patients.AnyAsync(p => p.CompanyId == company.Id, ct);
        if (hasPatients)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;

        var patients = new[]
        {
            new Patient
            {
                CompanyId = company.Id,
                FirstName = "Emma",
                LastName = "Kask",
                DateOfBirth = new DateOnly(1991, 4, 12),
                Email = "emma.kask@example.local",
                CreatedBy = createdBy
            },
            new Patient
            {
                CompanyId = company.Id,
                FirstName = "Karl",
                LastName = "Tamm",
                DateOfBirth = new DateOnly(1987, 9, 21),
                Email = "karl.tamm@example.local",
                CreatedBy = createdBy
            },
            new Patient
            {
                CompanyId = company.Id,
                FirstName = "Liis",
                LastName = "Saar",
                DateOfBirth = new DateOnly(2000, 2, 3),
                Email = "liis.saar@example.local",
                CreatedBy = createdBy
            }
        };

        var rooms = new[]
        {
            new TreatmentRoom { CompanyId = company.Id, Name = "Room A", CreatedBy = createdBy },
            new TreatmentRoom { CompanyId = company.Id, Name = "Room B", CreatedBy = createdBy }
        };

        var dentists = new[]
        {
            new Dentist { CompanyId = company.Id, Name = "Dr. Laura Vaher", LicenseNumber = "EST-10021", CreatedBy = createdBy },
            new Dentist { CompanyId = company.Id, Name = "Dr. Martin Ots", LicenseNumber = "EST-10022", CreatedBy = createdBy }
        };

        var treatmentTypes = new[]
        {
            new TreatmentType { CompanyId = company.Id, Name = "Checkup", DurationMinutes = 30, Price = 55m, CreatedBy = createdBy },
            new TreatmentType { CompanyId = company.Id, Name = "Root Canal", DurationMinutes = 90, Price = 420m, CreatedBy = createdBy },
            new TreatmentType { CompanyId = company.Id, Name = "Crown", DurationMinutes = 75, Price = 680m, CreatedBy = createdBy }
        };

        var insurancePlans = new[]
        {
            new InsurancePlan { CompanyId = company.Id, Name = "Haigekassa Standard", CoverageType = "Statutory", CreatedBy = createdBy },
            new InsurancePlan { CompanyId = company.Id, Name = "SmilePlus Private", CoverageType = "Private", CreatedBy = createdBy }
        };

        await db.Patients.AddRangeAsync(patients, ct);
        await db.TreatmentRooms.AddRangeAsync(rooms, ct);
        await db.Dentists.AddRangeAsync(dentists, ct);
        await db.TreatmentTypes.AddRangeAsync(treatmentTypes, ct);
        await db.InsurancePlans.AddRangeAsync(insurancePlans, ct);

        var plans = new[]
        {
            new TreatmentPlan
            {
                CompanyId = company.Id,
                PatientId = patients[0].Id,
                Title = "Urgent pain relief + restorative follow-up",
                CreatedBy = createdBy
            },
            new TreatmentPlan
            {
                CompanyId = company.Id,
                PatientId = patients[1].Id,
                Title = "Elective restorative package",
                CreatedBy = createdBy
            }
        };

        await db.TreatmentPlans.AddRangeAsync(plans, ct);

        var planItems = new[]
        {
            new PlanItem
            {
                CompanyId = company.Id,
                TreatmentPlanId = plans[0].Id,
                Description = "Emergency endodontic treatment",
                EstimatedCost = 420m,
                Sequence = 1,
                Urgency = 5,
                DecisionStatus = PlanItemDecisionStatus.Accepted,
                CreatedBy = createdBy
            },
            new PlanItem
            {
                CompanyId = company.Id,
                TreatmentPlanId = plans[0].Id,
                Description = "Ceramic crown placement",
                EstimatedCost = 680m,
                Sequence = 2,
                Urgency = 3,
                DecisionStatus = PlanItemDecisionStatus.Deferred,
                CreatedBy = createdBy
            },
            new PlanItem
            {
                CompanyId = company.Id,
                TreatmentPlanId = plans[1].Id,
                Description = "Composite filling bundle",
                EstimatedCost = 290m,
                Sequence = 1,
                Urgency = 2,
                DecisionStatus = PlanItemDecisionStatus.Pending,
                CreatedBy = createdBy
            }
        };

        var appointments = new[]
        {
            new Appointment
            {
                CompanyId = company.Id,
                PatientId = patients[0].Id,
                TreatmentRoomId = rooms[0].Id,
                DentistId = dentists[0].Id,
                PlanItemId = planItems[0].Id,
                StartAt = now.AddDays(1).AddHours(2),
                EndAt = now.AddDays(1).AddHours(3),
                TypeName = "Root Canal",
                CreatedBy = createdBy
            },
            new Appointment
            {
                CompanyId = company.Id,
                PatientId = patients[1].Id,
                TreatmentRoomId = rooms[1].Id,
                DentistId = dentists[1].Id,
                PlanItemId = planItems[2].Id,
                StartAt = now.AddDays(2).AddHours(1),
                EndAt = now.AddDays(2).AddHours(2),
                TypeName = "Checkup",
                CreatedBy = createdBy
            },
            new Appointment
            {
                CompanyId = company.Id,
                PatientId = patients[2].Id,
                TreatmentRoomId = rooms[0].Id,
                DentistId = dentists[0].Id,
                StartAt = now.AddDays(3).AddHours(3),
                EndAt = now.AddDays(3).AddHours(4),
                TypeName = "Crown",
                CreatedBy = createdBy
            }
        };

        var toothRecords = new[]
        {
            new ToothRecord { CompanyId = company.Id, PatientId = patients[0].Id, ToothNumber = 14, ConditionStatus = "Caries", Notes = "Deep cavity", CreatedBy = createdBy },
            new ToothRecord { CompanyId = company.Id, PatientId = patients[0].Id, ToothNumber = 15, ConditionStatus = "Post-op", Notes = "Needs crown", CreatedBy = createdBy },
            new ToothRecord { CompanyId = company.Id, PatientId = patients[1].Id, ToothNumber = 7, ConditionStatus = "Healthy", Notes = "Monitor", CreatedBy = createdBy },
            new ToothRecord { CompanyId = company.Id, PatientId = patients[1].Id, ToothNumber = 8, ConditionStatus = "Caries", Notes = "Small lesion", CreatedBy = createdBy },
            new ToothRecord { CompanyId = company.Id, PatientId = patients[2].Id, ToothNumber = 30, ConditionStatus = "Restored", Notes = "Composite", CreatedBy = createdBy },
            new ToothRecord { CompanyId = company.Id, PatientId = patients[2].Id, ToothNumber = 31, ConditionStatus = "Healthy", Notes = null, CreatedBy = createdBy }
        };

        var xrays = new[]
        {
            new Xray
            {
                CompanyId = company.Id,
                PatientId = patients[0].Id,
                TakenAt = now.AddDays(-120),
                FileUrl = "https://example.local/xrays/emma-kask-1.png",
                CreatedBy = createdBy
            },
            new Xray
            {
                CompanyId = company.Id,
                PatientId = patients[1].Id,
                TakenAt = now.AddDays(-200),
                FileUrl = "https://example.local/xrays/karl-tamm-1.png",
                CreatedBy = createdBy
            }
        };

        var treatments = new[]
        {
            new Treatment
            {
                CompanyId = company.Id,
                PatientId = patients[0].Id,
                TreatmentTypeId = treatmentTypes[1].Id,
                PerformedAt = now.AddDays(-10),
                Cost = 420m,
                CreatedBy = createdBy
            },
            new Treatment
            {
                CompanyId = company.Id,
                PatientId = patients[2].Id,
                TreatmentTypeId = treatmentTypes[0].Id,
                PerformedAt = now.AddDays(-5),
                Cost = 55m,
                CreatedBy = createdBy
            }
        };

        var estimates = new[]
        {
            new CostEstimate
            {
                CompanyId = company.Id,
                PatientId = patients[0].Id,
                InsurancePlanId = insurancePlans[0].Id,
                CountryTemplate = "US-DEFAULT",
                TotalAmount = 1100m,
                ClaimStatus = "Submitted",
                CreatedBy = createdBy
            },
            new CostEstimate
            {
                CompanyId = company.Id,
                PatientId = patients[1].Id,
                InsurancePlanId = insurancePlans[1].Id,
                CountryTemplate = "DE-Kostenvoranschlag",
                TotalAmount = 860m,
                ClaimStatus = "Draft",
                CreatedBy = createdBy
            }
        };

        var invoices = new[]
        {
            new Invoice { CompanyId = company.Id, PatientId = patients[0].Id, Amount = 1100m, IsPaid = false, CreatedBy = createdBy },
            new Invoice { CompanyId = company.Id, PatientId = patients[1].Id, Amount = 860m, IsPaid = false, CreatedBy = createdBy },
            new Invoice { CompanyId = company.Id, PatientId = patients[2].Id, Amount = 120m, IsPaid = true, CreatedBy = createdBy }
        };

        await db.PlanItems.AddRangeAsync(planItems, ct);
        await db.Appointments.AddRangeAsync(appointments, ct);
        await db.ToothRecords.AddRangeAsync(toothRecords, ct);
        await db.Xrays.AddRangeAsync(xrays, ct);
        await db.Treatments.AddRangeAsync(treatments, ct);
        await db.CostEstimates.AddRangeAsync(estimates, ct);
        await db.Invoices.AddRangeAsync(invoices, ct);

        await db.PaymentPlans.AddAsync(new PaymentPlan
        {
            CompanyId = company.Id,
            InvoiceId = invoices[1].Id,
            Months = 12,
            MonthlyAmount = 71.67m,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            CreatedBy = createdBy
        }, ct);

        await db.AuditLogs.AddAsync(new AuditLog
        {
            CompanyId = company.Id,
            EntityName = "Seed",
            EntityId = company.Id.ToString(),
            Action = "CreateDemoData",
            OldValues = null,
            NewValues = "Initial demo dataset created",
            ChangedByUserId = "system",
            ChangedAt = DateTimeOffset.UtcNow,
            CreatedBy = "system"
        }, ct);
    }
}
