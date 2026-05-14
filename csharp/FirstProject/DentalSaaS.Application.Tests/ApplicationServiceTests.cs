using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Application.Patients;
using DentalSaaS.Application.ToothRecords;
using DentalSaaS.Application.TreatmentPlans;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Shared.Enums;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Tests;

public sealed class ApplicationServiceTests
{
    [Fact]
    public async Task PatientService_OnlyReturnsCurrentTenantPatients()
    {
        var tenant = new TestTenantAccessor { Context = new CurrentTenantContext { IsResolved = true, CompanyId = Guid.NewGuid(), TenantSlug = "acme" } };
        var user = new TestUserAccessor();
        var patientRepo = new TestPatientRepository();
        var audit = new TestAuditRepository();
        var authorization = new AllowAllAuthorizationService();

        await patientRepo.AddAsync(new Patient { CompanyId = tenant.Context.CompanyId, FirstName = "A", LastName = "One", DateOfBirth = new DateOnly(1990, 1, 1), Email = "a@test" });
        await patientRepo.AddAsync(new Patient { CompanyId = Guid.NewGuid(), FirstName = "B", LastName = "Two", DateOfBirth = new DateOnly(1991, 1, 1), Email = "b@test" });

        var service = new PatientService(patientRepo, tenant, user, audit, authorization);

        var items = await service.ListAsync();

        Assert.Single(items);
        Assert.Equal("A One", items.Single().FullName);
    }

    [Fact]
    public async Task TreatmentPlanService_AcceptItem_UpdatesDecisionStatus()
    {
        var companyId = Guid.NewGuid();
        var tenant = new TestTenantAccessor { Context = new CurrentTenantContext { IsResolved = true, CompanyId = companyId, TenantSlug = "acme" } };
        var user = new TestUserAccessor();
        var audit = new TestAuditRepository();
        var authorization = new AllowAllAuthorizationService();
        var repo = new TestTreatmentPlanRepository();
        var appointments = new TestCompanyCrudRepository<Appointment>();

        var plan = new TreatmentPlan { CompanyId = companyId, PatientId = Guid.NewGuid(), Title = "Plan A" };
        var item = new PlanItem { CompanyId = companyId, TreatmentPlanId = plan.Id, Description = "Root canal", Sequence = 1, Urgency = 5, EstimatedCost = 200 };
        plan.Items.Add(item);
        await repo.AddAsync(plan);

        var service = new TreatmentPlanService(repo, appointments, tenant, user, audit, authorization);

        var result = await service.AcceptItemAsync(plan.Id, item.Id);

        Assert.True(result.IsSuccess);
        var updated = await repo.GetWithItemsAsync(companyId, plan.Id);
        Assert.NotNull(updated);
        Assert.Equal(PlanItemDecisionStatus.Accepted, updated!.Items.Single().DecisionStatus);
    }

    [Fact]
    public async Task ToothRecordService_RejectsToothNumberOutsideUniversalRange()
    {
        var companyId = Guid.NewGuid();
        var tenant = new TestTenantAccessor { Context = new CurrentTenantContext { IsResolved = true, CompanyId = companyId, TenantSlug = "acme" } };
        var user = new TestUserAccessor();
        var records = new TestCompanyCrudRepository<ToothRecord>();
        var patients = new TestPatientRepository();
        var audit = new TestAuditRepository();
        var authorization = new AllowAllAuthorizationService();

        var patient = new Patient { CompanyId = companyId, FirstName = "Emma", LastName = "Test", DateOfBirth = new DateOnly(1990, 1, 1), Email = "emma@test.local" };
        await patients.AddAsync(patient);

        var service = new ToothRecordService(records, patients, tenant, user, authorization, audit);
        var result = await service.CreateAsync(new CreateToothRecordRequest(patient.Id, 33, "Caries", null));

        Assert.True(result.IsFailure);
        Assert.Equal("Tooth number must be between 1 and 32.", result.Error);
    }

    [Fact]
    public void RoleAuthorizationService_Employee_CannotManageClinicalPlans()
    {
        var user = new TestUserAccessor();
        user.Set(new CurrentUserContext
        {
            UserId = "emp",
            Email = "emp@test.local",
            ActiveTenantRole = Roles.CompanyEmployee,
            Roles = [Roles.CompanyEmployee]
        });

        var auth = new RoleAuthorizationService(user);
        Assert.True(auth.EnsureCanManageClinicalPlans().IsFailure);
    }

    private sealed class TestTenantAccessor : ICurrentTenantAccessor
    {
        public CurrentTenantContext Context { get; set; } = new() { IsResolved = false };
        public CurrentTenantContext Current => Context;
        public void Set(CurrentTenantContext context) => Context = context;
    }

    private sealed class TestUserAccessor : ICurrentUserAccessor
    {
        public CurrentUserContext Current { get; private set; } = new() { UserId = "user-1", Email = "u@test" };
        public void Set(CurrentUserContext context) => Current = context;
    }

    private sealed class TestAuditRepository : IAuditLogRepository
    {
        public Task AddAsync(AuditLog log, CancellationToken ct = default) => Task.CompletedTask;
    }

    private sealed class AllowAllAuthorizationService : IRoleAuthorizationService
    {
        public Result EnsureCanViewOperationalData() => Result.Success();
        public Result EnsureCanCreateOperationalData() => Result.Success();
        public Result EnsureCanEditOperationalData(string? entityCreatedBy) => Result.Success();
        public Result EnsureCanDeleteOperationalData(string? entityCreatedBy) => Result.Success();
        public Result EnsureCanManageUsers() => Result.Success();
        public Result EnsureCanManageCompanySettings() => Result.Success();
        public Result EnsureCanOperateBasicRecords() => Result.Success();
        public Result EnsureCanManageClinicalPlans() => Result.Success();
        public Result EnsureCanManageSubscription() => Result.Success();
        public Result EnsureCanTransferOwnership() => Result.Success();
        public Result EnsureCanViewReports() => Result.Success();
        public Result EnsureCanManageInsuranceRelationships() => Result.Success();
        public Result EnsureCanManageFinancialData() => Result.Success();
    }

    private sealed class TestPatientRepository : IPatientRepository
    {
        private readonly List<Patient> _items = [];

        public Task<IReadOnlyCollection<Patient>> ListByCompanyAsync(Guid companyId, CancellationToken ct = default)
            => Task.FromResult((IReadOnlyCollection<Patient>)_items.Where(i => i.CompanyId == companyId && !i.IsDeleted).ToArray());

        public Task<Patient?> GetAsync(Guid companyId, Guid id, CancellationToken ct = default)
            => Task.FromResult(_items.SingleOrDefault(i => i.CompanyId == companyId && i.Id == id && !i.IsDeleted));

        public Task AddAsync(Patient patient, CancellationToken ct = default)
        {
            _items.Add(patient);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Patient patient, CancellationToken ct = default) => Task.CompletedTask;

        public Task SoftDeleteAsync(Patient patient, string userId, CancellationToken ct = default)
        {
            patient.IsDeleted = true;
            return Task.CompletedTask;
        }
    }

    private sealed class TestTreatmentPlanRepository : ITreatmentPlanRepository
    {
        private readonly List<TreatmentPlan> _plans = [];

        public Task<TreatmentPlan?> GetWithItemsAsync(Guid companyId, Guid planId, CancellationToken ct = default)
            => Task.FromResult(_plans.SingleOrDefault(p => p.CompanyId == companyId && p.Id == planId && !p.IsDeleted));

        public Task<IReadOnlyCollection<TreatmentPlan>> ListAsync(Guid companyId, CancellationToken ct = default)
            => Task.FromResult((IReadOnlyCollection<TreatmentPlan>)_plans.Where(p => p.CompanyId == companyId && !p.IsDeleted).ToArray());

        public Task AddAsync(TreatmentPlan plan, CancellationToken ct = default)
        {
            _plans.Add(plan);
            return Task.CompletedTask;
        }

        public Task AddItemAsync(PlanItem item, CancellationToken ct = default)
        {
            var plan = _plans.SingleOrDefault(p => p.Id == item.TreatmentPlanId && p.CompanyId == item.CompanyId && !p.IsDeleted);
            if (plan is not null)
            {
                plan.Items.Add(item);
            }

            return Task.CompletedTask;
        }

        public Task UpdateItemAsync(PlanItem item, CancellationToken ct = default) => Task.CompletedTask;

        public Task UpdateAsync(TreatmentPlan plan, CancellationToken ct = default) => Task.CompletedTask;
    }

    private sealed class TestCompanyCrudRepository<TEntity> : ICompanyCrudRepository<TEntity>
        where TEntity : DentalSaaS.Domain.Common.CompanyEntityBase
    {
        private readonly List<TEntity> _items = [];

        public Task<IReadOnlyCollection<TEntity>> ListAsync(Guid companyId, CancellationToken ct = default)
            => Task.FromResult((IReadOnlyCollection<TEntity>)_items.Where(i => i.CompanyId == companyId && !i.IsDeleted).ToArray());

        public Task<TEntity?> GetAsync(Guid companyId, Guid id, CancellationToken ct = default)
            => Task.FromResult(_items.SingleOrDefault(i => i.CompanyId == companyId && i.Id == id && !i.IsDeleted));

        public Task AddAsync(TEntity entity, CancellationToken ct = default)
        {
            _items.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(TEntity entity, CancellationToken ct = default) => Task.CompletedTask;

        public Task SoftDeleteAsync(TEntity entity, string deletedBy, CancellationToken ct = default)
        {
            entity.IsDeleted = true;
            return Task.CompletedTask;
        }
    }
}
