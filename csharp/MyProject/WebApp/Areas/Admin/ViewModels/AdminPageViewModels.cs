using WebApp.ViewModels;

namespace WebApp.Areas.Admin.ViewModels;

public class AdminListPageViewModel<TItem> : IPageViewModel
{
    public string PageTitle { get; set; } = default!;
    public string Heading { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string CreateButtonText { get; set; } = default!;
    public IReadOnlyList<TItem> Items { get; set; } = Array.Empty<TItem>();
}

public class AdminHomeViewModel : IPageViewModel
{
    public string PageTitle { get; set; } = default!;
    public string Heading { get; set; } = default!;
    public string Description { get; set; } = default!;
    public IReadOnlyList<AdminSectionCardViewModel> Sections { get; set; } = Array.Empty<AdminSectionCardViewModel>();
}

public class AdminSectionCardViewModel
{
    public string Heading { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Area { get; set; } = "Admin";
    public string Controller { get; set; } = default!;
    public string Action { get; set; } = "Index";
    public string ButtonText { get; set; } = "Open";
}

public class LookupEditPageViewModel : IPageViewModel
{
    public string PageTitle { get; set; } = default!;
    public string Heading { get; set; } = default!;
    public string Description { get; set; } = default!;
    public LookupEditViewModel Form { get; set; } = new();
}

public class CurrencyEditPageViewModel : IPageViewModel
{
    public string PageTitle { get; set; } = default!;
    public string Heading { get; set; } = default!;
    public string Description { get; set; } = default!;
    public CurrencyEditViewModel Form { get; set; } = new();
}

public class MarketDataProviderEditPageViewModel : IPageViewModel
{
    public string PageTitle { get; set; } = default!;
    public string Heading { get; set; } = default!;
    public string Description { get; set; } = default!;
    public MarketDataProviderEditViewModel Form { get; set; } = new();
}
