namespace DentalSaaS.Application.CompanySettings;

public sealed record CompanySettingsDto(Guid Id, string CountryCode, int XrayIntervalDays);

public sealed record UpdateCompanySettingsRequest(string CountryCode, int XrayIntervalDays);
