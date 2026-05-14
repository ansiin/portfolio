using DentalSaaS.Application.Appointments;
using DentalSaaS.Application.Patients;
using DentalSaaS.Application.PracticeSetup;
using DentalSaaS.Application.TreatmentPlans;
using DentalSaaS.Application.Xrays;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Web.ViewModels.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantStaff)]
[Route("{tenantSlug}/appointments")]
public sealed class AppointmentsController : Controller
{
    private readonly IAppointmentService _appointments;
    private readonly IPatientService _patients;
    private readonly IPracticeSetupService _practiceSetup;
    private readonly ITreatmentPlanService _treatmentPlans;
    private readonly IXrayService _xrays;

    public AppointmentsController(
        IAppointmentService appointments,
        IPatientService patients,
        IPracticeSetupService practiceSetup,
        ITreatmentPlanService treatmentPlans,
        IXrayService xrays)
    {
        _appointments = appointments;
        _patients = patients;
        _practiceSetup = practiceSetup;
        _treatmentPlans = treatmentPlans;
        _xrays = xrays;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        var items = await _appointments.ListAsync(ct);
        return View(items);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(string tenantSlug, Guid? planItemId, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        return View(new AppointmentFormViewModel
        {
            PlanItemId = planItemId,
            StartAt = DateTimeOffset.UtcNow.AddDays(1),
            EndAt = DateTimeOffset.UtcNow.AddDays(1).AddMinutes(30)
        });
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string tenantSlug, AppointmentFormViewModel model, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        await PopulatePatientsViewDataAsync(ct);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _appointments.CreateAsync(new CreateAppointmentRequest(
            model.PatientId,
            model.TreatmentRoomId,
            model.DentistId,
            model.PlanItemId,
            model.StartAt,
            model.EndAt,
            model.TypeName), ct);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Create failed.");
            return View(model);
        }

        TempData["Success"] = "Appointment created.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _appointments.DeleteAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Appointment deleted.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    private async Task PopulatePatientsViewDataAsync(CancellationToken ct)
    {
        var patients = await _patients.ListAsync(ct);
        var patientLookup = patients.ToDictionary(p => p.Id, p => p.FullName);
        ViewBag.PatientOptions = patients
            .OrderBy(p => p.FullName)
            .ToArray();
        ViewBag.PatientLookup = patientLookup;

        var rooms = await _practiceSetup.ListRoomsAsync(ct);
        ViewBag.RoomOptions = rooms
            .OrderBy(r => r.Name)
            .ToArray();
        ViewBag.RoomLookup = rooms.ToDictionary(r => r.Id, r => r.Name);

        var dentists = await _practiceSetup.ListDentistsAsync(ct);
        ViewBag.DentistOptions = dentists
            .OrderBy(d => d.Name)
            .ToArray();
        ViewBag.DentistLookup = dentists.ToDictionary(d => d.Id, d => d.Name);

        var plans = await _treatmentPlans.ListAsync(ct);
        ViewBag.PlanItemOptions = plans
            .SelectMany(p => p.Items.Select(i =>
            {
                var patientName = patientLookup.TryGetValue(p.PatientId, out var name)
                    ? name
                    : "Unknown patient";
                return new SelectListItem(
                    $"{patientName} | {p.Title} | Seq {i.Sequence} | Urgency {i.Urgency} | {i.Description}",
                    i.Id.ToString());
            }))
            .ToArray();

        var overdueByPatient = await _xrays.GetPatientOverdueDaysAsync(ct);
        ViewBag.PatientOverdueDays = overdueByPatient;
        ViewBag.XrayOverdueSummary = await _xrays.GetOverdueSummaryAsync(ct);
    }
}
