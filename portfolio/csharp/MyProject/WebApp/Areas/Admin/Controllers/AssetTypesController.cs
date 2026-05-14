using App.BLL.Services;
using App.DTO.Admin.Lookups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Areas.Admin.ViewModels;
using WebApp.Helpers;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class AssetTypesController : Controller
{
    private readonly AdminLookupService _adminLookupService;

    public AssetTypesController(AdminLookupService adminLookupService)
    {
        _adminLookupService = adminLookupService;
    }

    public async Task<IActionResult> Index()
    {
        return View(new AdminListPageViewModel<AdminLookupDto>
        {
            PageTitle = UiText.T("AssetTypes"),
            Heading = UiText.T("AssetTypes"),
            Description = "Maintain asset categories available to users.",
            CreateButtonText = UiText.T("Create"),
            Items = await _adminLookupService.GetAssetTypesAsync()
        });
    }

    public IActionResult Create()
    {
        return View("~/Areas/Admin/Views/Shared/LookupForm.cshtml", CreateLookupPageModel("Create Asset Type", new LookupEditViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] LookupEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Areas/Admin/Views/Shared/LookupForm.cshtml", CreateLookupPageModel("Create Asset Type", vm));
        }

        try
        {
            await _adminLookupService.CreateAssetTypeAsync(new AdminLookupCreateDto
            {
                Code = vm.Code,
                DisplayNameEn = vm.DisplayNameEn,
                DisplayNameEt = vm.DisplayNameEt,
                IsActive = vm.IsActive
            });

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("~/Areas/Admin/Views/Shared/LookupForm.cshtml", CreateLookupPageModel("Create Asset Type", vm));
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _adminLookupService.GetAssetTypeAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        return View("~/Areas/Admin/Views/Shared/LookupForm.cshtml", CreateLookupPageModel("Edit Asset Type", new LookupEditViewModel
        {
            Id = entity.Id,
            Code = entity.Code,
            DisplayNameEn = entity.DisplayNameEn,
            DisplayNameEt = entity.DisplayNameEt,
            IsActive = entity.IsActive
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind(Prefix = "Form")] LookupEditViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View("~/Areas/Admin/Views/Shared/LookupForm.cshtml", CreateLookupPageModel("Edit Asset Type", vm));
        }

        try
        {
            var updated = await _adminLookupService.UpdateAssetTypeAsync(id, new AdminLookupUpdateDto
            {
                Code = vm.Code,
                DisplayNameEn = vm.DisplayNameEn,
                DisplayNameEt = vm.DisplayNameEt,
                IsActive = vm.IsActive
            });

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("~/Areas/Admin/Views/Shared/LookupForm.cshtml", CreateLookupPageModel("Edit Asset Type", vm));
        }
    }

    private static LookupEditPageViewModel CreateLookupPageModel(string title, LookupEditViewModel form)
    {
        return new LookupEditPageViewModel
        {
            PageTitle = title,
            Heading = title,
            Description = "Maintain localized lookup values used across the investing tracker.",
            Form = form
        };
    }
}
