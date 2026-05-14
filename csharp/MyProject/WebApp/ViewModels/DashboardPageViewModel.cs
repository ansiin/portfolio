using App.DTO.v1.Dashboard;

namespace WebApp.ViewModels;

public class DashboardPageViewModel : IPageViewModel
{
    public string PageTitle { get; init; } = default!;
    public string Heading { get; init; } = default!;
    public string Description { get; init; } = default!;
    public DashboardSummaryDto Summary { get; init; } = default!;
    public IReadOnlyList<DashboardAllocationItemDto> Allocation { get; init; } = Array.Empty<DashboardAllocationItemDto>();
    public IReadOnlyList<DashboardTimelinePointDto> Timeline { get; init; } = Array.Empty<DashboardTimelinePointDto>();
}
