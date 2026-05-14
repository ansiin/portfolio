using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Areas.Admin.ViewModels;
using WebApp.Helpers;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View(new AdminHomeViewModel
        {
            PageTitle = UiText.T("Admin"),
            Heading = UiText.T("Admin"),
            Description = "Maintain the system-level data, translations, and access-related overviews required by the investing tracker.",
            Sections = new[]
            {
                new AdminSectionCardViewModel
                {
                    Heading = UiText.T("Users"),
                    Description = "Review registered users and their assigned roles.",
                    Controller = "Users",
                    ButtonText = UiText.T("Open")
                },
                new AdminSectionCardViewModel
                {
                    Heading = UiText.T("Currencies"),
                    Description = "Manage base currencies, symbols and localized names.",
                    Controller = "Currencies",
                    ButtonText = UiText.T("Open")
                },
                new AdminSectionCardViewModel
                {
                    Heading = UiText.T("AssetTypes"),
                    Description = "Define the categories available for tracked assets.",
                    Controller = "AssetTypes",
                    ButtonText = UiText.T("Open")
                },
                new AdminSectionCardViewModel
                {
                    Heading = UiText.T("Exchanges"),
                    Description = "Maintain supported exchanges and their localized display names.",
                    Controller = "Exchanges",
                    ButtonText = UiText.T("Open")
                },
                new AdminSectionCardViewModel
                {
                    Heading = UiText.T("Providers"),
                    Description = "Manage market data providers and their optional base URLs.",
                    Controller = "MarketDataProviders",
                    ButtonText = UiText.T("Open")
                }
            }
        });
    }
}
