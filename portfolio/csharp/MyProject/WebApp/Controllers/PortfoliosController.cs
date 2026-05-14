using App.BLL.Services;
using App.DAL.EF;
using App.DTO.v1.Portfolios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class PortfoliosController : Controller
{
    private readonly PortfolioService _portfolioService;
    private readonly AppDbContext _context;

    public PortfoliosController(PortfolioService portfolioService, AppDbContext context)
    {
        _portfolioService = portfolioService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var portfolios = await _portfolioService.GetMyPortfoliosAsync();
        return View(portfolios);
    }

    public async Task<IActionResult> Create()
    {
        return View(new PortfolioCreateViewModel
        {
            CurrencySelectList = await BuildCurrencySelectListAsync()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PortfolioCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.CurrencySelectList = await BuildCurrencySelectListAsync(vm.BaseCurrencyId);
            return View(vm);
        }

        try
        {
            await _portfolioService.CreateAsync(new PortfolioCreateDto
            {
                Name = vm.Name,
                BaseCurrencyId = vm.BaseCurrencyId
            });

            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(nameof(vm.BaseCurrencyId), ex.Message);
            vm.CurrencySelectList = await BuildCurrencySelectListAsync(vm.BaseCurrencyId);
            return View(vm);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var portfolio = await _portfolioService.GetMyPortfolioAsync(id);
        if (portfolio == null)
        {
            return NotFound();
        }

        return View(new PortfolioEditViewModel
        {
            Id = portfolio.Id,
            Name = portfolio.Name,
            BaseCurrencyId = portfolio.BaseCurrencyId,
            IsArchived = portfolio.IsArchived,
            CurrencySelectList = await BuildCurrencySelectListAsync(portfolio.BaseCurrencyId)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, PortfolioEditViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            vm.CurrencySelectList = await BuildCurrencySelectListAsync(vm.BaseCurrencyId);
            return View(vm);
        }

        try
        {
            var updated = await _portfolioService.UpdateAsync(id, new PortfolioUpdateDto
            {
                Name = vm.Name,
                BaseCurrencyId = vm.BaseCurrencyId,
                IsArchived = vm.IsArchived
            });

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(nameof(vm.BaseCurrencyId), ex.Message);
        }

        vm.CurrencySelectList = await BuildCurrencySelectListAsync(vm.BaseCurrencyId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _portfolioService.DeleteAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> BuildCurrencySelectListAsync(Guid? selectedValue = null)
    {
        var currencies = await _context.Currencies
            .Where(currency => currency.IsActive)
            .OrderBy(currency => currency.Code)
            .ToListAsync();

        return currencies
            .Select(currency => new SelectListItem
            {
                Value = currency.Id.ToString(),
                Text = $"{currency.Code} - {currency.DisplayName.Translate() ?? currency.Code}",
                Selected = selectedValue == currency.Id
            })
            .ToList();
    }
}
