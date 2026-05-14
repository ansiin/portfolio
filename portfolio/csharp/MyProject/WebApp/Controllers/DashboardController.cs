using App.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardPageViewModel
        {
            PageTitle = UiText.T("Dashboard"),
            Heading = UiText.T("Dashboard"),
            Description = UiText.T("DashboardLead"),
            Summary = await _dashboardService.GetSummaryAsync(),
            Allocation = await _dashboardService.GetAllocationAsync(),
            Timeline = await _dashboardService.GetTimelineAsync()
        };

        return View(model);
    }
}
