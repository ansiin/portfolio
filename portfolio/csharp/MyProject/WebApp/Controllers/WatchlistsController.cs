using App.BLL.Services;
using App.DTO.v1.Watchlists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Helpers;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class WatchlistsController : Controller
{
    private readonly WatchlistService _watchlistService;
    private readonly AssetService _assetService;

    public WatchlistsController(WatchlistService watchlistService, AssetService assetService)
    {
        _watchlistService = watchlistService;
        _assetService = assetService;
    }

    public async Task<IActionResult> Index()
    {
        var watchlists = await _watchlistService.GetMyWatchlistsAsync();
        return View(watchlists);
    }

    public IActionResult Create()
    {
        return View(new WatchlistCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WatchlistCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var created = await _watchlistService.CreateAsync(new WatchlistCreateDto
        {
            Name = vm.Name
        });

        return RedirectToAction(nameof(Edit), new { id = created.Id });
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var watchlist = await _watchlistService.GetMyWatchlistAsync(id);
        if (watchlist == null)
        {
            return NotFound();
        }

        var vm = new WatchlistEditViewModel
        {
            Id = watchlist.Id,
            Name = watchlist.Name,
            Items = watchlist.Items,
            AssetSelectList = await BuildAssetSelectListAsync(watchlist.Items.Select(item => item.AssetId))
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, WatchlistEditViewModel vm)
    {
        if (id != vm.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            var current = await _watchlistService.GetMyWatchlistAsync(id);
            vm.Items = current?.Items ?? Array.Empty<WatchlistItemDto>();
            vm.AssetSelectList = await BuildAssetSelectListAsync(vm.Items.Select(item => item.AssetId), vm.AssetId);
            return View(vm);
        }

        var updated = await _watchlistService.UpdateAsync(id, new WatchlistUpdateDto
        {
            Name = vm.Name
        });

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(Guid id, Guid? assetId)
    {
        if (assetId == null || assetId == Guid.Empty)
        {
            TempData["Error"] = UiText.T("SelectAssetBeforeAdd");
            return RedirectToAction(nameof(Edit), new { id });
        }

        try
        {
            await _watchlistService.AddItemAsync(id, new WatchlistItemCreateDto
            {
                AssetId = assetId.Value
            });
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveItem(Guid id, Guid itemId)
    {
        await _watchlistService.RemoveItemAsync(id, itemId);
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _watchlistService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> BuildAssetSelectListAsync(IEnumerable<Guid> excludedAssetIds, Guid? selectedValue = null)
    {
        var excluded = excludedAssetIds.ToHashSet();
        var assets = await _assetService.GetMyAssetsAsync();

        var result = new List<SelectListItem>
        {
            new() { Value = string.Empty, Text = UiText.T("SelectAsset"), Selected = selectedValue == null }
        };

        result.AddRange(assets
            .Where(asset => (asset.IsActive && !excluded.Contains(asset.Id)) || asset.Id == selectedValue)
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
}
