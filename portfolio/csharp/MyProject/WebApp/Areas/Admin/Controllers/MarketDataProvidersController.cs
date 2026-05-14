using App.BLL.Services;
using App.DTO.Admin.Lookups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Areas.Admin.ViewModels;
using WebApp.Helpers;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class MarketDataProvidersController : Controller
{
    private readonly AdminLookupService _adminLookupService;

    public MarketDataProvidersController(AdminLookupService adminLookupService)
    {
        _adminLookupService = adminLookupService;
    }

    public async Task<IActionResult> Index()
    {
        return View(new AdminListPageViewModel<AdminMarketDataProviderDto>
        {
            PageTitle = UiText.T("MarketDataProviders"),
            Heading = UiText.T("MarketDataProviders"),
            Description = "Maintain provider metadata for manual and future automated pricing flows.",
            CreateButtonText = UiText.T("Create"),
            Items = await _adminLookupService.GetMarketDataProvidersAsync()
        });
    }

    public IActionResult Create()
    {
        return View("~/Areas/Admin/Views/Shared/MarketDataProviderForm.cshtml", CreateProviderPageModel("Create Market Data Provider", new MarketDataProviderEditViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] MarketDataProviderEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Areas/Admin/Views/Shared/MarketDataProviderForm.cshtml", CreateProviderPageModel("Create Market Data Provider", vm));
        }

        try
        {
            await _adminLookupService.CreateMarketDataProviderAsync(new AdminMarketDataProviderCreateDto
            {
                Code = vm.Code,
                DisplayNameEn = vm.DisplayNameEn,
                DisplayNameEt = vm.DisplayNameEt,
                BaseUrl = vm.BaseUrl,
                IsActive = vm.IsActive
            });

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("~/Areas/Admin/Views/Shared/MarketDataProviderForm.cshtml", CreateProviderPageModel("Create Market Data Provider", vm));
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _adminLookupService.GetMarketDataProviderAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        return View("~/Areas/Admin/Views/Shared/MarketDataProviderForm.cshtml", CreateProviderPageModel("Edit Market Data Provider", new MarketDataProviderEditViewModel
        {
            Id = entity.Id,
            Code = entity.Code,
            DisplayNameEn = entity.DisplayNameEn,
            DisplayNameEt = entity.DisplayNameEt,
            BaseUrl = entity.BaseUrl,
            IsActive = entity.IsActive
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind(Prefix = "Form")] MarketDataProviderEditViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View("~/Areas/Admin/Views/Shared/MarketDataProviderForm.cshtml", CreateProviderPageModel("Edit Market Data Provider", vm));
        }

        try
        {
            var updated = await _adminLookupService.UpdateMarketDataProviderAsync(id, new AdminMarketDataProviderUpdateDto
            {
                Code = vm.Code,
                DisplayNameEn = vm.DisplayNameEn,
                DisplayNameEt = vm.DisplayNameEt,
                BaseUrl = vm.BaseUrl,
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
            return View("~/Areas/Admin/Views/Shared/MarketDataProviderForm.cshtml", CreateProviderPageModel("Edit Market Data Provider", vm));
        }
    }

    private static MarketDataProviderEditPageViewModel CreateProviderPageModel(string title, MarketDataProviderEditViewModel form)
    {
        return new MarketDataProviderEditPageViewModel
        {
            PageTitle = title,
            Heading = title,
            Description = "Maintain localized provider metadata and optional external base URLs.",
            Form = form
        };
    }
}
