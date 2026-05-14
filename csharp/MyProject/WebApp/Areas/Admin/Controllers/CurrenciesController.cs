using App.BLL.Services;
using App.DTO.Admin.Lookups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Areas.Admin.ViewModels;
using WebApp.Helpers;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class CurrenciesController : Controller
{
    private readonly AdminLookupService _adminLookupService;

    public CurrenciesController(AdminLookupService adminLookupService)
    {
        _adminLookupService = adminLookupService;
    }

    public async Task<IActionResult> Index()
    {
        return View(new AdminListPageViewModel<AdminCurrencyDto>
        {
            PageTitle = UiText.T("Currencies"),
            Heading = UiText.T("Currencies"),
            Description = "Maintain supported base and asset currencies.",
            CreateButtonText = UiText.T("Create"),
            Items = await _adminLookupService.GetCurrenciesAsync()
        });
    }

    public IActionResult Create()
    {
        return View("~/Areas/Admin/Views/Shared/CurrencyForm.cshtml", CreateCurrencyPageModel("Create Currency", new CurrencyEditViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] CurrencyEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Areas/Admin/Views/Shared/CurrencyForm.cshtml", CreateCurrencyPageModel("Create Currency", vm));
        }

        try
        {
            await _adminLookupService.CreateCurrencyAsync(new AdminCurrencyCreateDto
            {
                Code = vm.Code,
                Symbol = vm.Symbol,
                DisplayNameEn = vm.DisplayNameEn,
                DisplayNameEt = vm.DisplayNameEt,
                IsActive = vm.IsActive
            });

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("~/Areas/Admin/Views/Shared/CurrencyForm.cshtml", CreateCurrencyPageModel("Create Currency", vm));
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _adminLookupService.GetCurrencyAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        return View("~/Areas/Admin/Views/Shared/CurrencyForm.cshtml", CreateCurrencyPageModel("Edit Currency", new CurrencyEditViewModel
        {
            Id = entity.Id,
            Code = entity.Code,
            Symbol = entity.Symbol,
            DisplayNameEn = entity.DisplayNameEn,
            DisplayNameEt = entity.DisplayNameEt,
            IsActive = entity.IsActive
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind(Prefix = "Form")] CurrencyEditViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View("~/Areas/Admin/Views/Shared/CurrencyForm.cshtml", CreateCurrencyPageModel("Edit Currency", vm));
        }

        try
        {
            var updated = await _adminLookupService.UpdateCurrencyAsync(id, new AdminCurrencyUpdateDto
            {
                Code = vm.Code,
                Symbol = vm.Symbol,
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
            return View("~/Areas/Admin/Views/Shared/CurrencyForm.cshtml", CreateCurrencyPageModel("Edit Currency", vm));
        }
    }

    private static CurrencyEditPageViewModel CreateCurrencyPageModel(string title, CurrencyEditViewModel form)
    {
        return new CurrencyEditPageViewModel
        {
            PageTitle = title,
            Heading = title,
            Description = "Maintain localized currency metadata used by portfolios and assets.",
            Form = form
        };
    }
}
