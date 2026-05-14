using DentalSaaS.Application.Billing;
using DentalSaaS.Application.Patients;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Web.ViewModels.Billing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantResolved)]
[Route("{tenantSlug}/billing")]
public sealed class BillingController : Controller
{
    private readonly IBillingService _billing;
    private readonly IPatientService _patients;

    public BillingController(IBillingService billing, IPatientService patients)
    {
        _billing = billing;
        _patients = patients;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        ViewBag.NewInvoice = new InvoiceFormViewModel();
        await PopulatePatientsViewDataAsync(ct);
        var items = await _billing.ListAsync(ct);
        return View(items);
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string tenantSlug, InvoiceFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid invoice form.";
            return RedirectToAction(nameof(Index), new { tenantSlug });
        }

        var result = await _billing.CreateAsync(new CreateInvoiceRequest(model.PatientId, model.Amount), ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Invoice created.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("{id:guid}/paid")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkPaid(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _billing.MarkPaidAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Invoice marked paid.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _billing.DeleteAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Invoice deleted.";
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
