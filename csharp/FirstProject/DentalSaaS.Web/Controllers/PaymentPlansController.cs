using DentalSaaS.Application.PaymentPlans;
using DentalSaaS.Application.Billing;
using DentalSaaS.Application.Patients;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Web.ViewModels.PaymentPlans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantResolved)]
[Route("{tenantSlug}/payment-plans")]
public sealed class PaymentPlansController : Controller
{
    private readonly IPaymentPlanService _paymentPlans;
    private readonly IBillingService _billing;
    private readonly IPatientService _patients;

    public PaymentPlansController(
        IPaymentPlanService paymentPlans,
        IBillingService billing,
        IPatientService patients)
    {
        _paymentPlans = paymentPlans;
        _billing = billing;
        _patients = patients;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        ViewBag.NewPlan = new PaymentPlanFormViewModel { StartDate = DateOnly.FromDateTime(DateTime.UtcNow) };
        await PopulateInvoicesViewDataAsync(ct);
        var items = await _paymentPlans.ListAsync(ct);
        return View(items);
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string tenantSlug, PaymentPlanFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid payment plan form.";
            return RedirectToAction(nameof(Index), new { tenantSlug });
        }

        var result = await _paymentPlans.CreateAsync(new CreatePaymentPlanRequest(
            model.InvoiceId,
            model.Months,
            model.MonthlyAmount,
            model.StartDate), ct);

        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Payment plan created.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [Authorize(Policy = Policies.TenantOwnerOrAdmin)]
    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _paymentPlans.DeleteAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Payment plan deleted.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    private async Task PopulateInvoicesViewDataAsync(CancellationToken ct)
    {
        var patients = await _patients.ListAsync(ct);
        var patientLookup = patients.ToDictionary(p => p.Id, p => p.FullName);

        var invoices = await _billing.ListAsync(ct);
        var invoiceOptions = invoices
            .OrderBy(i => i.IsPaid)
            .ThenByDescending(i => i.Amount)
            .Select(i =>
            {
                var patientName = patientLookup.TryGetValue(i.PatientId, out var fullName)
                    ? fullName
                    : "Unknown patient";

                var label = $"{patientName} - {i.Amount:0.00} ({(i.IsPaid ? "Paid" : "Unpaid")})";
                return new SelectListItem(label, i.Id.ToString());
            })
            .ToArray();

        ViewBag.InvoiceOptions = invoiceOptions;
        ViewBag.InvoiceLookup = invoices.ToDictionary(
            i => i.Id,
            i =>
            {
                var patientName = patientLookup.TryGetValue(i.PatientId, out var fullName)
                    ? fullName
                    : "Unknown patient";
                return $"{patientName} - {i.Amount:0.00} ({(i.IsPaid ? "Paid" : "Unpaid")})";
            });
    }
}
