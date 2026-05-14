using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Authorization;

public interface IRoleAuthorizationService
{
    Result EnsureCanViewOperationalData();
    Result EnsureCanCreateOperationalData();
    Result EnsureCanEditOperationalData(string? entityCreatedBy);
    Result EnsureCanDeleteOperationalData(string? entityCreatedBy);

    Result EnsureCanManageUsers();
    Result EnsureCanManageCompanySettings();
    Result EnsureCanOperateBasicRecords();
    Result EnsureCanManageClinicalPlans();
    Result EnsureCanManageSubscription();
    Result EnsureCanTransferOwnership();
    Result EnsureCanViewReports();
    Result EnsureCanManageInsuranceRelationships();
    Result EnsureCanManageFinancialData();
}
