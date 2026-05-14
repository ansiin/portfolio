using DentalSaaS.Application.Onboarding;
using DentalSaaS.Web.ViewModels.Onboarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[AllowAnonymous]
[Route("onboarding")]
public sealed class OnboardingController : Controller
{
    private readonly IOnboardingService _onboarding;

    public OnboardingController(IOnboardingService onboarding)
    {
        _onboarding = onboarding;
    }

    [HttpGet("")]
    public IActionResult Index()
        => View(new OnboardingViewModel());

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(OnboardingViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _onboarding.RegisterCompanyAsync(new OnboardingRequest(
            model.CompanyName,
            model.TenantSlug,
            model.OwnerEmail,
            model.OwnerPassword,
            model.OwnerDisplayName), ct);

        if (result.IsFailure || result.Value is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed.");
            return View(model);
        }

        TempData["Success"] = "Company created successfully. Please log in.";
        return Redirect("/account/login");
    }
}
