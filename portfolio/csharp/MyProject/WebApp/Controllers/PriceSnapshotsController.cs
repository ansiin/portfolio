using App.BLL.Services;
using App.DAL.EF;
using App.DTO.v1.PriceSnapshots;
using Base.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Helpers;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class PriceSnapshotsController : Controller
{
    private readonly PriceSnapshotService _priceSnapshotService;
    private readonly AssetService _assetService;
    private readonly AppDbContext _context;

    public PriceSnapshotsController(PriceSnapshotService priceSnapshotService, AssetService assetService, AppDbContext context)
    {
        _priceSnapshotService = priceSnapshotService;
        _assetService = assetService;
        _context = context;
    }

    public async Task<IActionResult> Index(Guid? assetId = null)
    {
        ViewBag.Assets = await BuildAssetFilterListAsync(assetId);
        ViewBag.SelectedAssetId = assetId;
        var snapshots = await _priceSnapshotService.GetMyPriceSnapshotsAsync(assetId);
        return View(snapshots);
    }

    public async Task<IActionResult> Create(Guid? assetId = null)
    {
        var vm = new PriceSnapshotCreateViewModel
        {
            AssetId = assetId ?? Guid.Empty,
            RecordedAt = DateTime.UtcNow
        };

        await PopulateCreateLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PriceSnapshotCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCreateLookupsAsync(vm);
            return View(vm);
        }

        try
        {
            await _priceSnapshotService.CreateAsync(new PriceSnapshotCreateDto
            {
                AssetId = vm.AssetId,
                CurrencyId = vm.CurrencyId,
                MarketDataProviderId = vm.MarketDataProviderId,
                RecordedAt = vm.RecordedAt,
                Price = vm.Price
            });

            return RedirectToAction(nameof(Index), new { assetId = vm.AssetId });
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
        var snapshot = await _priceSnapshotService.GetMyPriceSnapshotAsync(id);
        if (snapshot == null)
        {
            return NotFound();
        }

        var vm = new PriceSnapshotEditViewModel
        {
            Id = snapshot.Id,
            AssetId = snapshot.AssetId,
            CurrencyId = snapshot.CurrencyId,
            MarketDataProviderId = snapshot.MarketDataProviderId,
            RecordedAt = snapshot.RecordedAt,
            Price = snapshot.Price
        };

        await PopulateEditLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, PriceSnapshotEditViewModel vm)
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
            var updated = await _priceSnapshotService.UpdateAsync(id, new PriceSnapshotUpdateDto
            {
                AssetId = vm.AssetId,
                CurrencyId = vm.CurrencyId,
                MarketDataProviderId = vm.MarketDataProviderId,
                RecordedAt = vm.RecordedAt,
                Price = vm.Price
            });

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), new { assetId = vm.AssetId });
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
    public async Task<IActionResult> Delete(Guid id, Guid? assetId = null)
    {
        await _priceSnapshotService.DeleteAsync(id);
        return RedirectToAction(nameof(Index), new { assetId });
    }

    private async Task PopulateCreateLookupsAsync(PriceSnapshotCreateViewModel vm)
    {
        vm.AssetSelectList = await BuildAssetSelectListAsync(vm.AssetId);
        vm.CurrencySelectList = await BuildLookupSelectListAsync(_context.Currencies.Where(x => x.IsActive), vm.CurrencyId);
        vm.ProviderSelectList = await BuildLookupSelectListAsync(_context.MarketDataProviders.Where(x => x.IsActive), vm.MarketDataProviderId, true, UiText.T("NoProvider"));
    }

    private async Task PopulateEditLookupsAsync(PriceSnapshotEditViewModel vm)
    {
        vm.AssetSelectList = await BuildAssetSelectListAsync(vm.AssetId);
        vm.CurrencySelectList = await BuildLookupSelectListAsync(_context.Currencies.Where(x => x.IsActive), vm.CurrencyId);
        vm.ProviderSelectList = await BuildLookupSelectListAsync(_context.MarketDataProviders.Where(x => x.IsActive), vm.MarketDataProviderId, true, UiText.T("NoProvider"));
    }

    private async Task<List<SelectListItem>> BuildAssetSelectListAsync(Guid? selectedValue = null)
    {
        var assets = await _assetService.GetMyAssetsAsync();
        return assets
            .Where(asset => asset.IsActive || asset.Id == selectedValue)
            .OrderBy(asset => asset.Name)
            .Select(asset => new SelectListItem
            {
                Value = asset.Id.ToString(),
                Text = asset.Symbol is { Length: > 0 }
                    ? $"{asset.Name} ({asset.Symbol}) - {asset.PortfolioName}"
                    : $"{asset.Name} - {asset.PortfolioName}",
                Selected = selectedValue == asset.Id
            })
            .ToList();
    }

    private async Task<List<SelectListItem>> BuildAssetFilterListAsync(Guid? selectedValue = null)
    {
        var assets = await _assetService.GetMyAssetsAsync();
        var result = new List<SelectListItem>
        {
            new() { Value = string.Empty, Text = UiText.T("AllAssets"), Selected = selectedValue == null }
        };

        result.AddRange(assets
            .OrderBy(asset => asset.Name)
            .Select(asset => new SelectListItem
            {
                Value = asset.Id.ToString(),
                Text = asset.Symbol is { Length: > 0 }
                    ? $"{asset.Name} ({asset.Symbol}) - {asset.PortfolioName}"
                    : $"{asset.Name} - {asset.PortfolioName}",
                Selected = selectedValue == asset.Id
            }));

        return result;
    }

    private static async Task<List<SelectListItem>> BuildLookupSelectListAsync<TEntity>(
        IQueryable<TEntity> query,
        Guid? selectedValue = null,
        bool includeEmpty = false,
        string emptyText = "None")
        where TEntity : class
    {
        var items = await query.ToListAsync();
        var result = new List<SelectListItem>();

        if (includeEmpty)
        {
            result.Add(new SelectListItem
            {
                Value = string.Empty,
                Text = emptyText,
                Selected = selectedValue == null
            });
        }

        result.AddRange(items.Select(item =>
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
        }));

        return result;
    }
}
