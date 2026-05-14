using System.Security.Claims;
using DentalSaaS.Application.Abstractions;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Infrastructure.Persistence;
using DentalSaaS.Infrastructure.Seed;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Shared.Enums;
using DentalSaaS.Web.ViewModels.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.SystemOperator)]
[Route("system")]
public sealed class SystemController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly ISystemAdministrationService _system;
    private readonly UserManager<AppUser> _users;
    private readonly SignInManager<AppUser> _signIn;
    private readonly AppDbContext _db;

    public SystemController(
        IWebHostEnvironment environment,
        ISystemAdministrationService system,
        UserManager<AppUser> users,
        SignInManager<AppUser> signIn,
        AppDbContext db)
    {
        _environment = environment;
        _system = system;
        _users = users;
        _signIn = signIn;
        _db = db;
    }

    [HttpGet("")]
    public IActionResult Index()
        => View();

    [Authorize(Roles = Roles.SystemAdmin)]
    [HttpPost("demo/reseed-acme")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForceReseedAcme(CancellationToken ct)
    {
        if (!_environment.IsDevelopment())
        {
            TempData["Error"] = "Force reseed is only enabled in Development environment.";
            return RedirectToAction(nameof(Index));
        }

        await SeedDataService.ForceReseedAcmeAsync(HttpContext.RequestServices, ct);
        TempData["Success"] = "ACME demo data was reset and seeded again.";
        return Redirect("/acme/patients");
    }

    [Authorize(Roles = Roles.SystemAdmin + "," + Roles.SystemSupport)]
    [HttpGet("companies")]
    public async Task<IActionResult> Companies(CancellationToken ct)
    {
        var items = await _system.ListCompaniesAsync(ct);
        return View(items);
    }

    [Authorize(Roles = Roles.SystemAdmin)]
    [HttpPost("companies/{id:guid}/active")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetCompanyActive(Guid id, bool isActive, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var result = await _system.SetCompanyActiveAsync(id, isActive, userId, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Company status updated.";
        return RedirectToAction(nameof(Companies));
    }

    [Authorize(Roles = Roles.SystemAdmin + "," + Roles.SystemBilling)]
    [HttpGet("billing")]
    public async Task<IActionResult> Billing(CancellationToken ct)
    {
        ViewBag.Companies = await _system.ListCompaniesAsync(ct);
        var subscriptions = await _system.ListSubscriptionsAsync(ct);
        return View(subscriptions);
    }

    [Authorize(Roles = Roles.SystemAdmin + "," + Roles.SystemBilling)]
    [HttpPost("billing/tier")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeTier(Guid companyId, SubscriptionTier tier, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var result = await _system.ChangeSubscriptionTierAsync(companyId, tier, userId, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Tier updated.";
        return RedirectToAction(nameof(Billing));
    }

    [Authorize(Roles = Roles.SystemAdmin)]
    [HttpGet("impersonation")]
    public async Task<IActionResult> Impersonation(Guid? companyId, CancellationToken ct)
    {
        ViewBag.Companies = await _system.ListCompaniesAsync(ct);
        ViewBag.SelectedCompanyId = companyId;

        if (companyId.HasValue)
        {
            var members = await _db.CompanyMemberships
                .Where(m => m.CompanyId == companyId.Value)
                .OrderBy(m => m.Role)
                .ToListAsync(ct);

            ViewBag.CompanyMembers = members;

            var userIds = members
                .Select(m => Guid.TryParse(m.UserId, out var id) ? id : Guid.Empty)
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray();

            var userLookup = await _users.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(
                    u => u.Id.ToString(),
                    u => string.IsNullOrWhiteSpace(u.DisplayName)
                        ? (u.Email ?? u.UserName ?? u.Id.ToString())
                        : $"{u.DisplayName} ({u.Email ?? u.UserName})",
                    ct);

            ViewBag.MemberUserLookup = userLookup;
        }
        else
        {
            ViewBag.CompanyMembers = Array.Empty<CompanyMembership>();
            ViewBag.MemberUserLookup = new Dictionary<string, string>();
        }

        return View(new ImpersonationRequestViewModel { CompanyId = companyId ?? Guid.Empty });
    }

    [Authorize(Roles = Roles.SystemAdmin)]
    [HttpPost("impersonation/start")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartImpersonation(ImpersonationRequestViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Impersonation), new { companyId = model.CompanyId });
        }

        var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(adminUserId))
        {
            TempData["Error"] = "Current admin user is invalid.";
            return RedirectToAction(nameof(Impersonation), new { companyId = model.CompanyId });
        }

        var sessionResult = await _system.BeginImpersonationAsync(adminUserId, model.TargetUserId, model.CompanyId, model.Reason, ct);
        if (sessionResult.IsFailure || sessionResult.Value == Guid.Empty)
        {
            TempData["Error"] = sessionResult.Error ?? "Could not start impersonation.";
            return RedirectToAction(nameof(Impersonation), new { companyId = model.CompanyId });
        }

        var target = await _users.FindByIdAsync(model.TargetUserId);
        if (target is null)
        {
            TempData["Error"] = "Target user not found.";
            return RedirectToAction(nameof(Impersonation), new { companyId = model.CompanyId });
        }

        var extraClaims = new[]
        {
            new Claim("impersonating", "true"),
            new Claim("impersonator_user_id", adminUserId),
            new Claim("impersonation_session_id", sessionResult.Value.ToString())
        };

        await _signIn.SignInWithClaimsAsync(target, isPersistent: true, extraClaims);

        var company = await _db.Companies.SingleAsync(c => c.Id == model.CompanyId, ct);
        TempData["Success"] = $"Impersonating user {target.Email} in tenant '{company.Slug}'.";
        return Redirect($"/{company.Slug}/patients");
    }

    [Authorize(Policy = Policies.Impersonation)]
    [HttpPost("impersonation/stop")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StopImpersonation(CancellationToken ct)
    {
        var sessionClaim = User.FindFirst("impersonation_session_id")?.Value;
        if (Guid.TryParse(sessionClaim, out var sessionId))
        {
            await _system.EndImpersonationAsync(sessionId, ct);
        }

        var impersonatorId = User.FindFirst("impersonator_user_id")?.Value;
        if (string.IsNullOrWhiteSpace(impersonatorId))
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        var admin = await _users.FindByIdAsync(impersonatorId);
        if (admin is null)
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        await _signIn.SignInAsync(admin, isPersistent: true);
        TempData["Success"] = "Impersonation stopped.";
        return RedirectToAction(nameof(Index));
    }
}
