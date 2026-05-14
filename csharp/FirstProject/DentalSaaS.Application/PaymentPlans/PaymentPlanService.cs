using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Enums;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.PaymentPlans;

public sealed class PaymentPlanService : IPaymentPlanService
{
    private readonly ICompanyCrudRepository<PaymentPlan> _plans;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;
    private readonly IRoleAuthorizationService _authorization;
    private readonly IFeatureGateService _features;

    public PaymentPlanService(
        ICompanyCrudRepository<PaymentPlan> plans,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user,
        IRoleAuthorizationService authorization,
        IFeatureGateService features)
    {
        _plans = plans;
        _tenant = tenant;
        _user = user;
        _authorization = authorization;
        _features = features;
    }

    public async Task<IReadOnlyCollection<PaymentPlanDto>> ListAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanViewOperationalData().IsFailure)
        {
            return [];
        }

        var gate = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.PaymentPlan, ct);
        if (gate.IsFailure)
        {
            return [];
        }

        var items = await _plans.ListAsync(_tenant.Current.CompanyId, ct);
        return items
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentPlanDto(p.Id, p.InvoiceId, p.Months, p.MonthlyAmount, p.StartDate))
            .ToArray();
    }

    public async Task<Result<Guid>> CreateAsync(CreatePaymentPlanRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var gate = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.PaymentPlan, ct);
        if (gate.IsFailure)
        {
            return Result<Guid>.Failure(gate.Error ?? "Payment plans are not available.");
        }

        var permission = _authorization.EnsureCanManageFinancialData();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }

        if (request.Months <= 0 || request.MonthlyAmount <= 0)
        {
            return Result<Guid>.Failure("Months and monthly amount must be positive.");
        }

        var plan = new PaymentPlan
        {
            CompanyId = _tenant.Current.CompanyId,
            InvoiceId = request.InvoiceId,
            Months = request.Months,
            MonthlyAmount = request.MonthlyAmount,
            StartDate = request.StartDate,
            CreatedBy = _user.Current.UserId
        };

        await _plans.AddAsync(plan, ct);
        return Result<Guid>.Success(plan.Id);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        var gate = await _features.EnsureAllowedAsync(_tenant.Current.CompanyId, Feature.PaymentPlan, ct);
        if (gate.IsFailure)
        {
            return gate;
        }

        var permission = _authorization.EnsureCanManageFinancialData();
        if (permission.IsFailure)
        {
            return permission;
        }

        var plan = await _plans.GetAsync(_tenant.Current.CompanyId, id, ct);
        if (plan is null)
        {
            return Result.Failure("Payment plan not found.");
        }

        await _plans.SoftDeleteAsync(plan, _user.Current.UserId, ct);
        return Result.Success();
    }

    private void EnsureTenant()
    {
        if (!_tenant.Current.IsResolved)
        {
            throw new InvalidOperationException("Tenant is not resolved.");
        }
    }
}
