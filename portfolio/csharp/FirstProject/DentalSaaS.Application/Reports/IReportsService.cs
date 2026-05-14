namespace DentalSaaS.Application.Reports;

public interface IReportsService
{
    Task<ReportsDashboardDto> GetDashboardAsync(DateOnly? dateFrom = null, DateOnly? dateTo = null, CancellationToken ct = default);
}
