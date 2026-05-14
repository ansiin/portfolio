using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.CompanySettings;

public interface ICompanySettingsService
{
    Task<CompanySettingsDto?> GetAsync(CancellationToken ct = default);
    Task<Result> UpdateAsync(UpdateCompanySettingsRequest request, CancellationToken ct = default);
}
