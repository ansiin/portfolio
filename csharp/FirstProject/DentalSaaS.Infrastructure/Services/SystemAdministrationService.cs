using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Infrastructure.Persistence;
using DentalSaaS.Shared.Enums;
using DentalSaaS.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalSaaS.Infrastructure.Services;

public sealed class SystemAdministrationService : ISystemAdministrationService
{
    private readonly AppDbContext _db;

    public SystemAdministrationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyCollection<SystemCompanyItem>> ListCompaniesAsync(CancellationToken ct = default)
    {
        var companies = await _db.Companies
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

        return companies
            .Select(c => new SystemCompanyItem(c.Id, c.Name, c.Slug, c.Tier, c.IsActive))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<SystemSubscriptionItem>> ListSubscriptionsAsync(CancellationToken ct = default)
    {
        var companies = await _db.Companies.ToListAsync(ct);
        var subscriptions = await _db.Subscriptions.ToListAsync(ct);

        return subscriptions
            .Join(companies,
                sub => sub.CompanyId,
                company => company.Id,
                (sub, company) => new SystemSubscriptionItem(sub.CompanyId, company.Slug, sub.Tier, sub.ValidFrom))
            .OrderByDescending(s => s.ValidFrom)
            .ToArray();
    }

    public async Task<Result> SetCompanyActiveAsync(Guid companyId, bool isActive, string changedByUserId, CancellationToken ct = default)
    {
        var company = await _db.Companies.SingleOrDefaultAsync(c => c.Id == companyId, ct);
        if (company is null)
        {
            return Result.Failure("Company not found.");
        }

        company.IsActive = isActive;
        _db.Companies.Update(company);

        await _db.AuditLogs.AddAsync(new AuditLog
        {
            CompanyId = companyId,
            EntityName = "Company",
            EntityId = companyId.ToString(),
            Action = "ActivationChange",
            OldValues = (!isActive).ToString(),
            NewValues = isActive.ToString(),
            ChangedByUserId = changedByUserId,
            ChangedAt = DateTimeOffset.UtcNow,
            CreatedBy = changedByUserId
        }, ct);

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> ChangeSubscriptionTierAsync(Guid companyId, SubscriptionTier tier, string changedByUserId, CancellationToken ct = default)
    {
        var company = await _db.Companies.SingleOrDefaultAsync(c => c.Id == companyId, ct);
        if (company is null)
        {
            return Result.Failure("Company not found.");
        }

        company.Tier = tier;

        var existingSubscriptions = await _db.Subscriptions
            .Where(s => s.CompanyId == companyId)
            .ToListAsync(ct);

        var existing = existingSubscriptions
            .OrderByDescending(s => s.ValidFrom)
            .FirstOrDefault();

        if (existing is not null)
        {
            existing.IsDeleted = true;
            existing.DeletedAt = DateTimeOffset.UtcNow;
            existing.DeletedBy = changedByUserId;
            _db.Subscriptions.Update(existing);
        }

        await _db.Subscriptions.AddAsync(new Subscription
        {
            CompanyId = companyId,
            Tier = tier,
            CreatedBy = changedByUserId
        }, ct);

        await _db.AuditLogs.AddAsync(new AuditLog
        {
            CompanyId = companyId,
            EntityName = "Subscription",
            EntityId = companyId.ToString(),
            Action = "TierChange",
            OldValues = existing?.Tier.ToString(),
            NewValues = tier.ToString(),
            ChangedByUserId = changedByUserId,
            ChangedAt = DateTimeOffset.UtcNow,
            CreatedBy = changedByUserId
        }, ct);

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<Guid>> BeginImpersonationAsync(string adminUserId, string targetUserId, Guid companyId, string reason, CancellationToken ct = default)
    {
        var membershipExists = await _db.CompanyMemberships.AnyAsync(
            m => m.CompanyId == companyId && m.UserId == targetUserId, ct);
        if (!membershipExists)
        {
            return Result<Guid>.Failure("Target user is not a member of the selected company.");
        }

        var session = new ImpersonationSession
        {
            AdminUserId = adminUserId,
            TargetUserId = targetUserId,
            CompanyId = companyId,
            Reason = reason.Trim()
        };

        await _db.ImpersonationSessions.AddAsync(session, ct);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(session.Id);
    }

    public async Task EndImpersonationAsync(Guid sessionId, CancellationToken ct = default)
    {
        var session = await _db.ImpersonationSessions.SingleOrDefaultAsync(s => s.Id == sessionId, ct);
        if (session is null || session.EndedAt.HasValue)
        {
            return;
        }

        session.EndedAt = DateTimeOffset.UtcNow;
        _db.ImpersonationSessions.Update(session);
        await _db.SaveChangesAsync(ct);
    }
}
