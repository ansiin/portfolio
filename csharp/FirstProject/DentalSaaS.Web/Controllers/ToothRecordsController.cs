using DentalSaaS.Application.Patients;
using DentalSaaS.Application.ToothRecords;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Web.ViewModels.ToothRecords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantStaff)]
[Route("{tenantSlug}/tooth-records")]
public sealed class ToothRecordsController : Controller
{
    private readonly IToothRecordService _records;
    private readonly IPatientService _patients;

    public ToothRecordsController(IToothRecordService records, IPatientService patients)
    {
        _records = records;
        _patients = patients;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        var items = await _records.ListAsync(ct);
        return View(items);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        return View(new ToothRecordFormViewModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string tenantSlug, ToothRecordFormViewModel model, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _records.CreateAsync(new CreateToothRecordRequest(
            model.PatientId,
            model.ToothNumber,
            model.ConditionStatus,
            model.Notes), ct);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Create failed.");
            return View(model);
        }

        TempData["Success"] = "Tooth record created.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(string tenantSlug, Guid id, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);

        var item = await _records.GetAsync(id, ct);
        if (item is null)
        {
            return NotFound();
        }

        return View(new ToothRecordFormViewModel
        {
            Id = item.Id,
            PatientId = item.PatientId,
            ToothNumber = item.ToothNumber,
            ConditionStatus = item.ConditionStatus,
            Notes = item.Notes
        });
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string tenantSlug, Guid id, ToothRecordFormViewModel model, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _records.UpdateAsync(new UpdateToothRecordRequest(
            id,
            model.ToothNumber,
            model.ConditionStatus,
            model.Notes), ct);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Update failed.");
            return View(model);
        }

        TempData["Success"] = "Tooth record updated.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _records.DeleteAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Tooth record deleted.";
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
