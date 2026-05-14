using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.Authorization;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.PracticeSetup;

public sealed class PracticeSetupService : IPracticeSetupService
{
    private readonly ICompanyCrudRepository<TreatmentRoom> _rooms;
    private readonly ICompanyCrudRepository<TreatmentType> _types;
    private readonly ICompanyCrudRepository<Dentist> _dentists;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;
    private readonly IRoleAuthorizationService _authorization;

    public PracticeSetupService(
        ICompanyCrudRepository<TreatmentRoom> rooms,
        ICompanyCrudRepository<TreatmentType> types,
        ICompanyCrudRepository<Dentist> dentists,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user,
        IRoleAuthorizationService authorization)
    {
        _rooms = rooms;
        _types = types;
        _dentists = dentists;
        _tenant = tenant;
        _user = user;
        _authorization = authorization;
    }

    public async Task<IReadOnlyCollection<TreatmentRoomDto>> ListRoomsAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanViewOperationalData().IsFailure)
        {
            return [];
        }

        var rooms = await _rooms.ListAsync(_tenant.Current.CompanyId, ct);
        return rooms.Select(r => new TreatmentRoomDto(r.Id, r.Name)).ToArray();
    }

    public async Task<IReadOnlyCollection<TreatmentTypeDto>> ListTypesAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanViewOperationalData().IsFailure)
        {
            return [];
        }

        var types = await _types.ListAsync(_tenant.Current.CompanyId, ct);
        return types.Select(t => new TreatmentTypeDto(t.Id, t.Name, t.DurationMinutes, t.Price)).ToArray();
    }

    public async Task<IReadOnlyCollection<DentistDto>> ListDentistsAsync(CancellationToken ct = default)
    {
        EnsureTenant();
        if (_authorization.EnsureCanViewOperationalData().IsFailure)
        {
            return [];
        }

        var dentists = await _dentists.ListAsync(_tenant.Current.CompanyId, ct);
        return dentists.Select(d => new DentistDto(d.Id, d.Name, d.LicenseNumber)).ToArray();
    }

    public async Task<Result<Guid>> CreateRoomAsync(string name, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanManageCompanySettings();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Guid>.Failure("Room name is required.");
        }

        var room = new TreatmentRoom
        {
            CompanyId = _tenant.Current.CompanyId,
            Name = name.Trim(),
            CreatedBy = _user.Current.UserId
        };

        await _rooms.AddAsync(room, ct);
        return Result<Guid>.Success(room.Id);
    }

    public async Task<Result<Guid>> CreateTypeAsync(string name, int durationMinutes, decimal price, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanManageCompanySettings();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Guid>.Failure("Treatment type name is required.");
        }
        if (durationMinutes <= 0)
        {
            return Result<Guid>.Failure("Duration must be greater than 0.");
        }
        if (price <= 0)
        {
            return Result<Guid>.Failure("Price must be greater than 0.");
        }

        var type = new TreatmentType
        {
            CompanyId = _tenant.Current.CompanyId,
            Name = name.Trim(),
            DurationMinutes = durationMinutes,
            Price = price,
            CreatedBy = _user.Current.UserId
        };

        await _types.AddAsync(type, ct);
        return Result<Guid>.Success(type.Id);
    }

    public async Task<Result<Guid>> CreateDentistAsync(string name, string licenseNumber, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanManageCompanySettings();
        if (permission.IsFailure)
        {
            return Result<Guid>.Failure(permission.Error ?? "Forbidden.");
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Guid>.Failure("Dentist name is required.");
        }
        if (string.IsNullOrWhiteSpace(licenseNumber))
        {
            return Result<Guid>.Failure("License number is required.");
        }

        var dentist = new Dentist
        {
            CompanyId = _tenant.Current.CompanyId,
            Name = name.Trim(),
            LicenseNumber = licenseNumber.Trim(),
            CreatedBy = _user.Current.UserId
        };

        await _dentists.AddAsync(dentist, ct);
        return Result<Guid>.Success(dentist.Id);
    }

    public async Task<Result> DeleteRoomAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanManageCompanySettings();
        if (permission.IsFailure)
        {
            return permission;
        }

        var item = await _rooms.GetAsync(_tenant.Current.CompanyId, id, ct);
        if (item is null)
        {
            return Result.Failure("Room not found.");
        }

        await _rooms.SoftDeleteAsync(item, _user.Current.UserId, ct);
        return Result.Success();
    }

    public async Task<Result> DeleteTypeAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanManageCompanySettings();
        if (permission.IsFailure)
        {
            return permission;
        }

        var item = await _types.GetAsync(_tenant.Current.CompanyId, id, ct);
        if (item is null)
        {
            return Result.Failure("Treatment type not found.");
        }

        await _types.SoftDeleteAsync(item, _user.Current.UserId, ct);
        return Result.Success();
    }

    public async Task<Result> DeleteDentistAsync(Guid id, CancellationToken ct = default)
    {
        EnsureTenant();
        var permission = _authorization.EnsureCanManageCompanySettings();
        if (permission.IsFailure)
        {
            return permission;
        }

        var item = await _dentists.GetAsync(_tenant.Current.CompanyId, id, ct);
        if (item is null)
        {
            return Result.Failure("Dentist not found.");
        }

        await _dentists.SoftDeleteAsync(item, _user.Current.UserId, ct);
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
