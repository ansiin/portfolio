using DentalSaaS.Application.Patients;
using DentalSaaS.Application.Xrays;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Web.ViewModels.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantStaff)]
[Route("{tenantSlug}/patients")]
public sealed class PatientsController : Controller
{
    private readonly IPatientService _patients;
    private readonly IXrayService _xrays;

    public PatientsController(IPatientService patients, IXrayService xrays)
    {
        _patients = patients;
        _xrays = xrays;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        var items = await _patients.ListAsync(ct);
        ViewBag.PatientOverdueDays = await _xrays.GetPatientOverdueDaysAsync(ct);
        ViewBag.XrayOverdueSummary = await _xrays.GetOverdueSummaryAsync(ct);
        return View(items);
    }

    [HttpGet("create")]
    public IActionResult Create(string tenantSlug)
    {
        ViewBag.TenantSlug = tenantSlug;
        return View(new PatientFormViewModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string tenantSlug, PatientFormViewModel model, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _patients.CreateAsync(new CreatePatientRequest(
            model.FirstName,
            model.LastName,
            model.DateOfBirth,
            model.Email), ct);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Create failed.");
            return View(model);
        }

        TempData["Success"] = "Patient created.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(string tenantSlug, Guid id, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        var item = await _patients.GetAsync(id, ct);
        if (item is null)
        {
            return NotFound();
        }

        return View(new PatientFormViewModel
        {
            Id = item.Id,
            FirstName = item.FullName.Split(' ').FirstOrDefault() ?? string.Empty,
            LastName = item.FullName.Split(' ').Skip(1).FirstOrDefault() ?? string.Empty,
            DateOfBirth = item.DateOfBirth,
            Email = item.Email
        });
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string tenantSlug, Guid id, PatientFormViewModel model, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _patients.UpdateAsync(new UpdatePatientRequest(
            id,
            model.FirstName,
            model.LastName,
            model.DateOfBirth,
            model.Email), ct);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Update failed.");
            return View(model);
        }

        TempData["Success"] = "Patient updated.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string tenantSlug, Guid id, CancellationToken ct)
    {
        await _patients.DeleteAsync(id, ct);
        TempData["Success"] = "Patient deleted.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }
}
