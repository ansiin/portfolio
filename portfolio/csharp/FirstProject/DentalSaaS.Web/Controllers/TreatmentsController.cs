using DentalSaaS.Application.Patients;
using DentalSaaS.Application.PracticeSetup;
using DentalSaaS.Application.Treatments;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Web.ViewModels.Treatments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantStaff)]
[Route("{tenantSlug}/treatments")]
public sealed class TreatmentsController : Controller
{
    private readonly ITreatmentService _treatments;
    private readonly IPatientService _patients;
    private readonly IPracticeSetupService _practiceSetup;

    public TreatmentsController(
        ITreatmentService treatments,
        IPatientService patients,
        IPracticeSetupService practiceSetup)
    {
        _treatments = treatments;
        _patients = patients;
        _practiceSetup = practiceSetup;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulateOptionsAsync(ct);
        var items = await _treatments.ListAsync(ct);
        return View(items);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulateOptionsAsync(ct);
        return View(new TreatmentFormViewModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string tenantSlug, TreatmentFormViewModel model, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulateOptionsAsync(ct);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _treatments.CreateAsync(new CreateTreatmentRequest(
            model.PatientId,
            model.TreatmentTypeId,
            model.PerformedAt,
            model.Cost), ct);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Create failed.");
            return View(model);
        }

        TempData["Success"] = "Treatment recorded.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(string tenantSlug, Guid id, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulateOptionsAsync(ct);
        var item = await _treatments.GetAsync(id, ct);
        if (item is null)
        {
            return NotFound();
        }

        return View(new TreatmentFormViewModel
        {
            Id = item.Id,
            PatientId = item.PatientId,
            TreatmentTypeId = item.TreatmentTypeId,
            PerformedAt = item.PerformedAt,
            Cost = item.Cost
        });
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string tenantSlug, Guid id, TreatmentFormViewModel model, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulateOptionsAsync(ct);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _treatments.UpdateAsync(new UpdateTreatmentRequest(
            id,
            model.TreatmentTypeId,
            model.PerformedAt,
            model.Cost), ct);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Update failed.");
            return View(model);
        }

        TempData["Success"] = "Treatment updated.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _treatments.DeleteAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Treatment deleted.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    private async Task PopulateOptionsAsync(CancellationToken ct)
    {
        var patients = await _patients.ListAsync(ct);
        ViewBag.PatientOptions = patients.OrderBy(p => p.FullName).ToArray();
        ViewBag.PatientLookup = patients.ToDictionary(p => p.Id, p => p.FullName);

        var types = await _practiceSetup.ListTypesAsync(ct);
        ViewBag.TreatmentTypeOptions = types.OrderBy(t => t.Name).ToArray();
        ViewBag.TreatmentTypeLookup = types.ToDictionary(t => t.Id, t => t.Name);
    }
}
