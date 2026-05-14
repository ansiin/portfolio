using DentalSaaS.Application.Patients;
using DentalSaaS.Application.TreatmentPlans;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Web.ViewModels.TreatmentPlans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantStaff)]
[Route("{tenantSlug}/treatment-plans")]
public sealed class TreatmentPlansController : Controller
{
    private readonly IPatientService _patients;
    private readonly ITreatmentPlanService _service;

    public TreatmentPlansController(ITreatmentPlanService service, IPatientService patients)
    {
        _service = service;
        _patients = patients;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        var plans = await _service.ListAsync(ct);
        return View(plans);
    }

    [Authorize(Policy = Policies.TenantLeadership)]
    [HttpGet("create")]
    public async Task<IActionResult> Create(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        return View(new TreatmentPlanCreateViewModel());
    }

    [Authorize(Policy = Policies.TenantLeadership)]
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string tenantSlug, TreatmentPlanCreateViewModel model, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _service.CreatePlanAsync(new CreateTreatmentPlanRequest(model.PatientId, model.Title), ct);
        if (result.IsFailure || result.Value == Guid.Empty)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Create failed.");
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { tenantSlug, id = result.Value });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(string tenantSlug, Guid id, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        var plan = await _service.GetAsync(id, ct);
        if (plan is null)
        {
            return NotFound();
        }

        ViewBag.AddItemModel = new AddPlanItemViewModel { PlanId = plan.Id };
        return View(plan);
    }

    [Authorize(Policy = Policies.TenantLeadership)]
    [HttpPost("{id:guid}/items")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(string tenantSlug, Guid id, AddPlanItemViewModel model, CancellationToken ct)
    {
        var result = await _service.AddItemAsync(new AddPlanItemRequest(
            id,
            model.Description,
            model.EstimatedCost,
            model.Sequence,
            model.Urgency), ct);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error;
        }

        return RedirectToAction(nameof(Details), new { tenantSlug, id });
    }

    [Authorize(Policy = Policies.TenantLeadership)]
    [HttpPost("{id:guid}/items/{itemId:guid}/accept")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptItem(string tenantSlug, Guid id, Guid itemId, CancellationToken ct)
    {
        await _service.AcceptItemAsync(id, itemId, ct);
        return RedirectToAction(nameof(Details), new { tenantSlug, id });
    }

    [Authorize(Policy = Policies.TenantLeadership)]
    [HttpPost("{id:guid}/items/{itemId:guid}/defer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeferItem(string tenantSlug, Guid id, Guid itemId, CancellationToken ct)
    {
        await _service.DeferItemAsync(id, itemId, ct);
        return RedirectToAction(nameof(Details), new { tenantSlug, id });
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
