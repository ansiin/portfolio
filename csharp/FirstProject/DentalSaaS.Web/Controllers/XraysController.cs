using DentalSaaS.Application.Patients;
using DentalSaaS.Application.Xrays;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Web.ViewModels.Xrays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantStaff)]
[Route("{tenantSlug}/xrays")]
public sealed class XraysController : Controller
{
    private readonly IXrayService _xrays;
    private readonly IPatientService _patients;

    public XraysController(IXrayService xrays, IPatientService patients)
    {
        _xrays = xrays;
        _patients = patients;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        ViewBag.OverdueSummary = await _xrays.GetOverdueSummaryAsync(ct);
        var items = await _xrays.ListAsync(ct);
        return View(items);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        return View(new XrayFormViewModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string tenantSlug, XrayFormViewModel model, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _xrays.CreateAsync(new CreateXrayRequest(model.PatientId, model.TakenAt, model.FileUrl), ct);
        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Create failed.");
            return View(model);
        }

        TempData["Success"] = "X-ray entry created.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(string tenantSlug, Guid id, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        var item = await _xrays.GetAsync(id, ct);
        if (item is null)
        {
            return NotFound();
        }

        return View(new XrayFormViewModel
        {
            Id = item.Id,
            PatientId = item.PatientId,
            TakenAt = item.TakenAt,
            FileUrl = item.FileUrl
        });
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string tenantSlug, Guid id, XrayFormViewModel model, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _xrays.UpdateAsync(new UpdateXrayRequest(id, model.TakenAt, model.FileUrl), ct);
        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Update failed.");
            return View(model);
        }

        TempData["Success"] = "X-ray entry updated.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _xrays.DeleteAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "X-ray entry deleted.";
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
