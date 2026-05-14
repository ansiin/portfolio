using WebApp.ViewModels;

namespace WebApp.Areas.Admin.ViewModels;

public class AdminUserOverviewViewModel : IPageViewModel
{
    public string PageTitle { get; set; } = default!;
    public string Heading { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int UserCount { get; set; }
    public int RoleCount { get; set; }
    public IReadOnlyList<AdminUserOverviewItemViewModel> Users { get; set; } = Array.Empty<AdminUserOverviewItemViewModel>();
}

public class AdminUserOverviewItemViewModel
{
    public string Email { get; set; } = default!;
    public string? UserName { get; set; }
    public bool EmailConfirmed { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}
