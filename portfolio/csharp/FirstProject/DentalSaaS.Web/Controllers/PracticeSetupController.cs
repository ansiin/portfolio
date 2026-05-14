using DentalSaaS.Application.PracticeSetup;
using DentalSaaS.Shared.Constants;
using DentalSaaS.Web.ViewModels.PracticeSetup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[Authorize(Policy = Policies.TenantOwnerOrAdmin)]
[Route("{tenantSlug}/practice-setup")]
public sealed class PracticeSetupController : Controller
{
    private readonly IPracticeSetupService _setup;

    public PracticeSetupController(IPracticeSetupService setup)
    {
        _setup = setup;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string tenantSlug, CancellationToken ct)
    {
        ViewBag.TenantSlug = tenantSlug;
        ViewBag.NewRoom = new RoomFormViewModel();
        ViewBag.NewType = new TreatmentTypeFormViewModel();
        ViewBag.NewDentist = new DentistFormViewModel();

        var rooms = await _setup.ListRoomsAsync(ct);
        var types = await _setup.ListTypesAsync(ct);
        var dentists = await _setup.ListDentistsAsync(ct);
        return View((rooms, types, dentists));
    }

    [HttpPost("rooms")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRoom(string tenantSlug, RoomFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Room name is required.";
            return RedirectToAction(nameof(Index), new { tenantSlug });
        }

        var result = await _setup.CreateRoomAsync(model.Name, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Treatment room created.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpPost("types")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateType(string tenantSlug, TreatmentTypeFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Treatment type input is invalid.";
            return RedirectToAction(nameof(Index), new { tenantSlug });
        }

        var result = await _setup.CreateTypeAsync(model.Name, model.DurationMinutes, model.Price, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Treatment type created.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpPost("dentists")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDentist(string tenantSlug, DentistFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Dentist name and license number are required.";
            return RedirectToAction(nameof(Index), new { tenantSlug });
        }

        var result = await _setup.CreateDentistAsync(model.Name, model.LicenseNumber, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Dentist created.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpPost("rooms/{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRoom(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _setup.DeleteRoomAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Room deleted.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpPost("types/{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteType(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _setup.DeleteTypeAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Type deleted.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }

    [HttpPost("dentists/{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDentist(string tenantSlug, Guid id, CancellationToken ct)
    {
        var result = await _setup.DeleteDentistAsync(id, ct);
        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.Error : "Dentist deleted.";
        return RedirectToAction(nameof(Index), new { tenantSlug });
    }
}
