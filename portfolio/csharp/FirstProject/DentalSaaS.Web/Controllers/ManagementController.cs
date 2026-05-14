using DentalSaaS.Application.Authorization;
using DentalSaaS.Application.Abstractions;
using DentalSaaS.Application.CompanySettings;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Shared.Enums;
using DentalSaaS.Web.ViewModels.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantResolved)]
[Route("{tenantSlug}/management")]
public sealed class ManagementController : Controller
{
    private readonly IRoleAuthorizationService _authorization;
    private readonly ICompanySettingsService _settings;
    private readonly ISystemAdministrationService _system;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ICurrentUserAccessor _user;

    public ManagementController(
        IRoleAuthorizationService authorization,
        ICompanySettingsService settings,
        ISystemAdministrationService system,
        ICurrentTenantAccessor tenant,
        ICurrentUserAccessor user)
    {
        _authorization = authorization;
        _settings = settings;
        _system = system;
        _tenant = tenant;
        _user = user;
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpGet("users")]
    public IActionResult Users(string tenantSlug)
    {
        var permission = _authorization.EnsureCanManageUsers();
        if (permission.IsFailure)
        {
            TempData["Error"] = permission.Error;
            return Redirect("/account/access-denied");
        }

        ViewBag.TenantSlug = tenantSlug;
        return View();
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpGet("settings")]
    public async Task<IActionResult> Settings(string tenantSlug, CancellationToken ct)
    {
        var permission = _authorization.EnsureCanManageCompanySettings();
        if (permission.IsFailure)
        {
            TempData["Error"] = permission.Error;
            return Redirect("/account/access-denied");
        }

        ViewBag.TenantSlug = tenantSlug;
        var settings = await _settings.GetAsync(ct);
        var model = new CompanySettingsFormViewModel
        {
            CountryCode = settings?.CountryCode ?? "US",
            XrayIntervalDays = settings?.XrayIntervalDays ?? 180
        };
        return View(model);
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("settings")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSettings(string tenantSlug, CompanySettingsFormViewModel model, CancellationToken ct)
    {
        var permission = _authorization.EnsureCanManageCompanySettings();
        if (permission.IsFailure)
        {
            TempData["Error"] = permission.Error;
            return Redirect("/account/access-denied");
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Settings input is invalid.";
            return RedirectToAction(nameof(Settings), new { tenantSlug });
        }

        var result = await _settings.UpdateAsync(new UpdateCompanySettingsRequest(model.CountryCode, model.XrayIntervalDays), ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Company settings updated.";
        return RedirectToAction(nameof(Settings), new { tenantSlug });
    }

    [Authorize(Policy = Policies.TenantOwner)]
    [HttpGet("subscription")]
    public async Task<IActionResult> Subscription(string tenantSlug, CancellationToken ct)
    {
        var permission = _authorization.EnsureCanManageSubscription();
        if (permission.IsFailure)
        {
            TempData["Error"] = permission.Error;
            return Redirect("/account/access-denied");
        }

        ViewBag.TenantSlug = tenantSlug;
        ViewBag.Subscriptions = await _system.ListSubscriptionsAsync(ct);
        return View();
    }

    [Authorize(Policy = Policies.TenantOwner)]
    [HttpPost("subscription")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSubscription(string tenantSlug, SubscriptionTier tier, CancellationToken ct)
    {
        var permission = _authorization.EnsureCanManageSubscription();
        if (permission.IsFailure)
        {
            TempData["Error"] = permission.Error;
            return Redirect("/account/access-denied");
        }

        var result = await _system.ChangeSubscriptionTierAsync(_tenant.Current.CompanyId, tier, _user.Current.UserId, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Subscription tier updated.";
        return RedirectToAction(nameof(Subscription), new { tenantSlug });
    }

    [Authorize(Policy = Policies.TenantOwner)]
    [HttpGet("ownership")]
    public IActionResult Ownership(string tenantSlug)
    {
        var permission = _authorization.EnsureCanTransferOwnership();
        if (permission.IsFailure)
        {
            TempData["Error"] = permission.Error;
            return Redirect("/account/access-denied");
        }

        ViewBag.TenantSlug = tenantSlug;
        return View();
    }
}
