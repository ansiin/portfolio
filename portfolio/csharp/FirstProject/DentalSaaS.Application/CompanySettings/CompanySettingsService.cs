using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Shared.Models;
using CompanySettingsEntity = DentalSaaS.Domain.Entities.CompanySettings;

namespace DentalSaaS.Application.CompanySettings;

public sealed class CompanySettingsService : ICompanySettingsService
{
    private readonly ICompanyCrudRepository<CompanySettingsEntity> _settings;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;
    private readonly IRoleAuthorizationService _authorization;

    public CompanySettingsService(
        ICompanyCrudRepository<CompanySettingsEntity> settings,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user,
        IRoleAuthorizationService authorization)
    {
        _settings = settings;
        _tenant = tenant;
        _user = user;
        _authorization = authorization;
    }

    public async Task<CompanySettingsDto?> GetAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanManageCompanySettings().IsFailure)
        {
            return null;
        }

        var item = (await _settings.ListAsync(_tenant.Current.CompanyId, ct))
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefault();

        return item is null
            ? null
            : new CompanySettingsDto(item.Id, item.CountryCode, item.XrayIntervalDays);
    }

    public async Task<Result> UpdateAsync(UpdateCompanySettingsRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanManageCompanySettings();
        if (permission.IsFailure)
        {
            return permission;
        }

        var countryCode = request.CountryCode?.Trim().ToUpperInvariant() ?? string.Empty;
        if (countryCode.Length is < 2 or > 3)
        {
            return Result.Failure("Country code must be 2-3 characters.");
        }

        if (request.XrayIntervalDays < 30 || request.XrayIntervalDays > 730)
        {
            return Result.Failure("X-ray interval must be between 30 and 730 days.");
        }

        var existing = (await _settings.ListAsync(_tenant.Current.CompanyId, ct))
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefault();

        if (existing is null)
        {
            await _settings.AddAsync(new CompanySettingsEntity
            {
                CompanyId = _tenant.Current.CompanyId,
                CountryCode = countryCode,
                XrayIntervalDays = request.XrayIntervalDays,
                CreatedBy = _user.Current.UserId
            }, ct);

            return Result.Success();
        }

        existing.CountryCode = countryCode;
        existing.XrayIntervalDays = request.XrayIntervalDays;
        existing.UpdatedAt = DateTimeOffset.UtcNow;
        existing.UpdatedBy = _user.Current.UserId;

        await _settings.UpdateAsync(existing, ct);
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
