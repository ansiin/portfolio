using DentalSaaS.Application.Abstractions;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Authorization;

public sealed class RoleAuthorizationService : IRoleAuthorizationService
{
    private readonly ICurrentUserAccessor _user;

    public RoleAuthorizationService(ICurrentUserAccessor user)
    {
        _user = user;
    }

    public Result EnsureCanViewOperationalData()
    {
        return IsRole(Roles.CompanyOwner, Roles.CompanyAdmin, Roles.CompanyManager, Roles.CompanyEmployee)
            ? Result.Success()
            : Result.Failure("You do not have permission to view operational data.");
    }

    public Result EnsureCanCreateOperationalData()
    {
        return IsRole(Roles.CompanyOwner, Roles.CompanyAdmin, Roles.CompanyManager, Roles.CompanyEmployee)
            ? Result.Success()
            : Result.Failure("You do not have permission to create this record.");
    }

    public Result EnsureCanEditOperationalData(string? entityCreatedBy)
    {
        if (IsRole(Roles.CompanyOwner, Roles.CompanyAdmin, Roles.CompanyManager))
        {
            return Result.Success();
        }

        if (IsRole(Roles.CompanyEmployee) && string.Equals(entityCreatedBy, _user.Current.UserId, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Success();
        }

        return Result.Failure("You do not have permission to edit this record.");
    }

    public Result EnsureCanDeleteOperationalData(string? entityCreatedBy)
    {
        if (IsRole(Roles.CompanyOwner, Roles.CompanyAdmin, Roles.CompanyManager))
        {
            return Result.Success();
        }

        return Result.Failure("You do not have permission to delete this record.");
    }

    public Result EnsureCanManageUsers()
    {
        return IsRole(Roles.CompanyOwner, Roles.CompanyAdmin)
            ? Result.Success()
            : Result.Failure("User and role management is allowed only for Owner/Admin.");
    }

    public Result EnsureCanManageCompanySettings()
    {
        return IsRole(Roles.CompanyOwner, Roles.CompanyAdmin)
            ? Result.Success()
            : Result.Failure("Company settings can be managed only by Owner/Admin.");
    }

    public Result EnsureCanOperateBasicRecords()
    {
        return IsRole(Roles.CompanyOwner, Roles.CompanyAdmin, Roles.CompanyManager, Roles.CompanyEmployee)
            ? Result.Success()
            : Result.Failure("You do not have permission to operate basic records.");
    }

    public Result EnsureCanManageClinicalPlans()
    {
        return IsRole(Roles.CompanyOwner, Roles.CompanyAdmin, Roles.CompanyManager)
            ? Result.Success()
            : Result.Failure("Clinical plan decisions are allowed for Owner/Admin/Manager.");
    }

    public Result EnsureCanManageSubscription()
    {
        return IsRole(Roles.CompanyOwner)
            ? Result.Success()
            : Result.Failure("Subscription and billing can be managed only by Owner.");
    }

    public Result EnsureCanTransferOwnership()
    {
        return IsRole(Roles.CompanyOwner)
            ? Result.Success()
            : Result.Failure("Ownership transfer can be managed only by Owner.");
    }

    public Result EnsureCanViewReports()
    {
        return IsRole(Roles.CompanyOwner, Roles.CompanyAdmin, Roles.CompanyManager)
            ? Result.Success()
            : Result.Failure("Reports are available for Owner/Admin/Manager.");
    }

    public Result EnsureCanManageInsuranceRelationships()
    {
        return IsRole(Roles.CompanyOwner, Roles.CompanyAdmin)
            ? Result.Success()
            : Result.Failure("Insurance relationships can be managed only by Owner/Admin.");
    }

    public Result EnsureCanManageFinancialData()
    {
        return IsRole(Roles.CompanyOwner, Roles.CompanyAdmin)
            ? Result.Success()
            : Result.Failure("Financial data can be managed only by Owner/Admin.");
    }

    private bool IsRole(params string[] allowed)
    {
        var role = _user.Current.ActiveTenantRole;
        return !string.IsNullOrWhiteSpace(role)
               && allowed.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
