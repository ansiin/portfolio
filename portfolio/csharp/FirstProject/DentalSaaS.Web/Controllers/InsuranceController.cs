using DentalSaaS.Application.Insurance;
using DentalSaaS.Application.Patients;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Shared.Enums;
using DentalSaaS.Web.ViewModels.Insurance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantResolved)]
[Route("{tenantSlug}/insurance")]
public sealed class InsuranceController : Controller
{
    private readonly IInsuranceService _insurance;
    private readonly IPatientService _patients;

    public InsuranceController(IInsuranceService insurance, IPatientService patients)
    {
        _insurance = insurance;
        _patients = patients;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        ViewBag.NewPlan = new InsurancePlanFormViewModel();
        ViewBag.NewEstimate = new CostEstimateFormViewModel();
        await PopulatePatientsViewDataAsync(ct);

        var plans = await _insurance.ListPlansAsync(ct);
        var estimates = await _insurance.ListEstimatesAsync(ct);
        return View((plans, estimates));
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("plans")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePlan(string tenantSlug, InsurancePlanFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Insurance plan name and coverage type are required.";
            return RedirectToAction(nameof(Index), new { tenantSlug });
        }

        var result = await _insurance.CreatePlanAsync(new CreateInsurancePlanRequest(model.Name, model.CoverageType), ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Insurance plan created.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("estimates")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEstimate(string tenantSlug, CostEstimateFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Estimate input is invalid.";
            return RedirectToAction(nameof(Index), new { tenantSlug });
        }

        var result = await _insurance.CreateEstimateAsync(new CreateCostEstimateRequest(
            model.PatientId,
            model.InsurancePlanId,
            model.CountryTemplate,
            model.TotalAmount), ct);

        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Cost estimate created.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("estimates/{id:guid}/status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateClaimStatus(string tenantSlug, Guid id, string claimStatus, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(claimStatus))
        {
            TempData["Error"] = "Claim status is required.";
            return RedirectToAction(nameof(Index), new { tenantSlug });
        }

        var result = await _insurance.UpdateClaimStatusAsync(id, claimStatus, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Claim status updated.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("estimates/{id:guid}/submit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitEstimate(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _insurance.SubmitEstimateAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Estimate submitted to provider.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("estimates/{id:guid}/submission-state")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetSubmissionState(string tenantSlug, Guid id, InsuranceSubmissionState state, string? message, CancellationToken ct)
    {
        var result = await _insurance.SetSubmissionStateAsync(id, state, message, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Submission state updated.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("plans/{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePlan(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _insurance.DeletePlanAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Insurance plan deleted.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("estimates/{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEstimate(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _insurance.DeleteEstimateAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Estimate deleted.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    private async Task PopulatePatientsViewDataAsync(CancellationToken ct)
    {
        var patients = await _patients.ListAsync(ct);
        ViewBag.PatientOptions = patients
            .OrderBy(p => p.FullName)
            .ToArray();

        ViewBag.PatientLookup = patients.ToDictionary(p => p.Id, p => p.FullName);
    }
}
