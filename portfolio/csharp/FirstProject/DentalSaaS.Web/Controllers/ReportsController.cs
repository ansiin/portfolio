using DentalSaaS.Application.Authorization;
using DentalSaaS.Application.Reports;
using DentalSaaS.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantResolved)]
[Route("{tenantSlug}/reports")]
public sealed class ReportsController : Controller
{
    private readonly IRoleAuthorizationService _authorization;
    private readonly IReportsService _reports;

    public ReportsController(IRoleAuthorizationService authorization, IReportsService reports)
    {
        _authorization = authorization;
        _reports = reports;
    }

    [Authorize(Policy = Policies.TenantLeadership)]
    [HttpGet("")]
    public async Task<IActionResult> Index(string tenantSlug, DateOnly? dateFrom, DateOnly? dateTo, CancellationToken ct)
    {
        var permission = _authorization.EnsureCanViewReports();
        if (permission.IsFailure)
        {
            TempData["Error"] = permission.Error;
            return Redirect("/account/access-denied");
        }

        ViewBag.TenantSlug = tenantSlug;
        var model = await _reports.GetDashboardAsync(dateFrom, dateTo, ct);
        return View(model);
    }
}
