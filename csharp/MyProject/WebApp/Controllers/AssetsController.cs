using App.BLL.Services;
using App.DAL.EF;
using App.DTO.v1.Assets;
using Base.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Helpers;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class AssetsController : Controller
{
    private readonly AssetService _assetService;
    private readonly PortfolioService _portfolioService;
    private readonly AppDbContext _context;

    public AssetsController(AssetService assetService, PortfolioService portfolioService, AppDbContext context)
    {
        _assetService = assetService;
        _portfolioService = portfolioService;
        _context = context;
    }

    public async Task<IActionResult> Index(Guid? portfolioId = null)
    {
        ViewBag.Portfolios = await BuildPortfolioFilterListAsync(portfolioId);
        ViewBag.SelectedPortfolioId = portfolioId;

        var assets = await _assetService.GetMyAssetsAsync(portfolioId);
        return View(assets);
    }

    public async Task<IActionResult> Create(Guid? portfolioId = null)
    {
        var vm = new AssetCreateViewModel
        {
            PortfolioId = portfolioId ?? Guid.Empty
        };

        await PopulateCreateLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssetCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCreateLookupsAsync(vm);
            return View(vm);
        }

        try
        {
            await _assetService.CreateAsync(new AssetCreateDto
            {
                PortfolioId = vm.PortfolioId,
                Name = vm.Name,
                Symbol = vm.Symbol,
                AssetTypeId = vm.AssetTypeId,
                CurrencyId = vm.CurrencyId,
                ExchangeId = vm.ExchangeId,
                MarketDataProviderId = vm.MarketDataProviderId
            });

            return RedirectToAction(nameof(Index), new { portfolioId = vm.PortfolioId });
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateCreateLookupsAsync(vm);
            return View(vm);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var asset = await _assetService.GetMyAssetAsync(id);
        if (asset == null)
        {
            return NotFound();
        }

        var vm = new AssetEditViewModel
        {
            Id = asset.Id,
            PortfolioId = asset.PortfolioId,
            Name = asset.Name,
            Symbol = asset.Symbol,
            AssetTypeId = asset.AssetTypeId,
            CurrencyId = asset.CurrencyId,
            ExchangeId = asset.ExchangeId,
            MarketDataProviderId = asset.MarketDataProviderId,
            IsActive = asset.IsActive
        };

        await PopulateEditLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AssetEditViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await PopulateEditLookupsAsync(vm);
            return View(vm);
        }

        try
        {
            var updated = await _assetService.UpdateAsync(id, new AssetUpdateDto
            {
                Name = vm.Name,
                Symbol = vm.Symbol,
                AssetTypeId = vm.AssetTypeId,
                CurrencyId = vm.CurrencyId,
                ExchangeId = vm.ExchangeId,
                MarketDataProviderId = vm.MarketDataProviderId,
                IsActive = vm.IsActive
            });

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), new { portfolioId = vm.PortfolioId });
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateEditLookupsAsync(vm);
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid? portfolioId = null)
    {
        await _assetService.DeactivateAsync(id);
        return RedirectToAction(nameof(Index), new { portfolioId });
    }

    private async Task PopulateCreateLookupsAsync(AssetCreateViewModel vm)
    {
        vm.PortfolioSelectList = await BuildPortfolioSelectListAsync(vm.PortfolioId);
        vm.AssetTypeSelectList = await BuildLookupSelectListAsync(_context.AssetTypes.Where(x => x.IsActive), vm.AssetTypeId);
        vm.CurrencySelectList = await BuildLookupSelectListAsync(_context.Currencies.Where(x => x.IsActive), vm.CurrencyId);
        vm.ExchangeSelectList = await BuildLookupSelectListAsync(_context.Exchanges.Where(x => x.IsActive), vm.ExchangeId);
        vm.ProviderSelectList = await BuildLookupSelectListAsync(_context.MarketDataProviders.Where(x => x.IsActive), vm.MarketDataProviderId);
    }

    private async Task PopulateEditLookupsAsync(AssetEditViewModel vm)
    {
        vm.AssetTypeSelectList = await BuildLookupSelectListAsync(_context.AssetTypes.Where(x => x.IsActive), vm.AssetTypeId);
        vm.CurrencySelectList = await BuildLookupSelectListAsync(_context.Currencies.Where(x => x.IsActive), vm.CurrencyId);
        vm.ExchangeSelectList = await BuildLookupSelectListAsync(_context.Exchanges.Where(x => x.IsActive), vm.ExchangeId);
        vm.ProviderSelectList = await BuildLookupSelectListAsync(_context.MarketDataProviders.Where(x => x.IsActive), vm.MarketDataProviderId);
    }

    private async Task<List<SelectListItem>> BuildPortfolioSelectListAsync(Guid? selectedValue = null)
    {
        var portfolios = await _portfolioService.GetMyPortfoliosAsync();
        return portfolios
            .Where(portfolio => !portfolio.IsArchived || portfolio.Id == selectedValue)
            .Select(portfolio => new SelectListItem
            {
                Value = portfolio.Id.ToString(),
                Text = portfolio.Name,
                Selected = selectedValue == portfolio.Id
            })
            .ToList();
    }

    private async Task<List<SelectListItem>> BuildPortfolioFilterListAsync(Guid? selectedValue = null)
    {
        var portfolios = await _portfolioService.GetMyPortfoliosAsync();
        var result = new List<SelectListItem>
        {
            new() { Value = string.Empty, Text = UiText.T("AllPortfolios"), Selected = selectedValue == null }
        };

        result.AddRange(portfolios.Select(portfolio => new SelectListItem
        {
            Value = portfolio.Id.ToString(),
            Text = portfolio.Name,
            Selected = selectedValue == portfolio.Id
        }));

        return result;
    }

    private static async Task<List<SelectListItem>> BuildLookupSelectListAsync<TEntity>(IQueryable<TEntity> query, Guid? selectedValue = null)
        where TEntity : class
    {
        var items = await query.ToListAsync();

        return items.Select(item =>
        {
            var id = (Guid)item.GetType().GetProperty("Id")!.GetValue(item)!;
            var code = (string)item.GetType().GetProperty("Code")!.GetValue(item)!;
            var langStr = (LangStr?)item.GetType().GetProperty("DisplayName")!.GetValue(item);
            var text = langStr?.Translate() ?? code;

            return new SelectListItem
            {
                Value = id.ToString(),
                Text = $"{code} - {text}",
                Selected = selectedValue == id
            };
        }).ToList();
    }
}
