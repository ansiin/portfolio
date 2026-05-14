using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Models;
using CompanySettingsEntity = DentalSaaS.Domain.Entities.CompanySettings;

namespace DentalSaaS.Application.Xrays;

public sealed class XrayService : IXrayService
{
    private readonly ICompanyCrudRepository<Xray> _xrays;
    private readonly ICompanyCrudRepository<CompanySettingsEntity> _settings;
    private readonly IPatientRepository _patients;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;
    private readonly IRoleAuthorizationService _authorization;

    public XrayService(
        ICompanyCrudRepository<Xray> xrays,
        ICompanyCrudRepository<CompanySettingsEntity> settings,
        IPatientRepository patients,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user,
        IRoleAuthorizationService authorization)
    {
        _xrays = xrays;
        _settings = settings;
        _patients = patients;
        _tenant = tenant;
        _user = user;
        _authorization = authorization;
    }

    public async Task<IReadOnlyCollection<XrayDto>> ListAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanOperateBasicRecords().IsFailure)
        {
            return [];
        }

        var intervalDays = await GetIntervalDaysAsync(ct);
        var now = DateTimeOffset.UtcNow;
        var items = await _xrays.ListAsync(_tenant.Current.CompanyId, ct);

        return items
            .OrderByDescending(x => x.TakenAt)
            .Select(x =>
            {
                var dueAt = x.TakenAt.AddDays(intervalDays);
                var isOverdue = dueAt < now;
                var overdueDays = isOverdue ? (int)Math.Floor((now - dueAt).TotalDays) : 0;
                return new XrayDto(x.Id, x.PatientId, x.TakenAt, dueAt, isOverdue, overdueDays, x.FileUrl);
            })
            .ToArray();
    }

    public async Task<XrayDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanOperateBasicRecords().IsFailure)
        {
            return null;
        }

        var intervalDays = await GetIntervalDaysAsync(ct);
        var now = DateTimeOffset.UtcNow;
        var xray = await _xrays.GetAsync(_tenant.Current.CompanyId, id, ct);
        if (xray is null)
        {
            return null;
        }

        var dueAt = xray.TakenAt.AddDays(intervalDays);
        var isOverdue = dueAt < now;
        var overdueDays = isOverdue ? (int)Math.Floor((now - dueAt).TotalDays) : 0;
        return new XrayDto(xray.Id, xray.PatientId, xray.TakenAt, dueAt, isOverdue, overdueDays, xray.FileUrl);
    }

    public async Task<Result<Guid>> CreateAsync(CreateXrayRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanOperateBasicRecords();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }

        if (request.PatientId == Guid.Empty)
        {
            return Result<Guid>.Failure("Patient is required.");
        }
        if (string.IsNullOrWhiteSpace(request.FileUrl))
        {
            return Result<Guid>.Failure("X-ray file URL is required.");
        }

        var patient = await _patients.GetAsync(_tenant.Current.CompanyId, request.PatientId, ct);
        if (patient is null)
        {
            return Result<Guid>.Failure("Selected patient was not found.");
        }

        var item = new Xray
        {
            CompanyId = _tenant.Current.CompanyId,
            PatientId = request.PatientId,
            TakenAt = request.TakenAt,
            FileUrl = request.FileUrl.Trim(),
            CreatedBy = _user.Current.UserId
        };

        await _xrays.AddAsync(item, ct);
        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result> UpdateAsync(UpdateXrayRequest request, CancellationToken ct = default)
    {
        EnsureTenant();
        var item = await _xrays.GetAsync(_tenant.Current.CompanyId, request.Id, ct);
        if (item is null)
        {
            return Result.Failure("X-ray not found.");
        }

        var permission = _authorization.EnsureCanOperateBasicRecords();
        if (permission.IsFailure)
        {
            return permission;
        }

        if (string.IsNullOrWhiteSpace(request.FileUrl))
        {
            return Result.Failure("X-ray file URL is required.");
        }

        item.TakenAt = request.TakenAt;
        item.FileUrl = request.FileUrl.Trim();
        item.UpdatedAt = DateTimeOffset.UtcNow;
        item.UpdatedBy = _user.Current.UserId;

        await _xrays.UpdateAsync(item, ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        var item = await _xrays.GetAsync(_tenant.Current.CompanyId, id, ct);
        if (item is null)
        {
            return Result.Failure("X-ray not found.");
        }

        var permission = _authorization.EnsureCanDeleteOperationalData(item.CreatedBy);
        if (permission.IsFailure)
        {
            return permission;
        }

        await _xrays.SoftDeleteAsync(item, _user.Current.UserId, ct);
        return Result.Success();
    }

    public async Task<XrayOverdueSummaryDto> GetOverdueSummaryAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        var intervalDays = await GetIntervalDaysAsync(ct);
        var now = DateTimeOffset.UtcNow;

        var patients = await _patients.ListByCompanyAsync(_tenant.Current.CompanyId, ct);
        var xrays = await _xrays.ListAsync(_tenant.Current.CompanyId, ct);

        var latestByPatient = xrays
            .GroupBy(x => x.PatientId)
            .ToDictionary(g => g.Key, g => g.MaxBy(x => x.TakenAt)!);

        var overdueCount = patients.Count(p =>
            latestByPatient.TryGetValue(p.Id, out var latest) &&
            latest.TakenAt.AddDays(intervalDays) < now);

        return new XrayOverdueSummaryDto(overdueCount, latestByPatient.Count, intervalDays);
    }

    public async Task<IReadOnlyDictionary<Guid, int>> GetPatientOverdueDaysAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        var intervalDays = await GetIntervalDaysAsync(ct);
        var now = DateTimeOffset.UtcNow;
        var xrays = await _xrays.ListAsync(_tenant.Current.CompanyId, ct);

        return xrays
            .GroupBy(x => x.PatientId)
            .Select(g =>
            {
                var latest = g.MaxBy(x => x.TakenAt)!;
                var due = latest.TakenAt.AddDays(intervalDays);
                var overdueDays = due < now ? (int)Math.Floor((now - due).TotalDays) : 0;
                return new { PatientId = g.Key, OverdueDays = overdueDays };
            })
            .Where(x => x.OverdueDays > 0)
            .ToDictionary(x => x.PatientId, x => x.OverdueDays);
    }

    private async Task<int> GetIntervalDaysAsync(CancellationToken ct)
    {
        var settings = (await _settings.ListAsync(_tenant.Current.CompanyId, ct))
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefault();

        return settings?.XrayIntervalDays > 0 ? settings.XrayIntervalDays : 180;
    }

    private void EnsureTenant()
    {
        if (!_tenant.Current.IsResolved)
        {
            throw new InvalidOperationException("Tenant is not resolved.");
        }
    }
}
