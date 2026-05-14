using App.BLL.Services;
using App.DTO.v1.PositionSnapshots;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Helpers;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class PositionSnapshotsController : Controller
{
    private readonly PositionSnapshotService _positionSnapshotService;
    private readonly PortfolioService _portfolioService;
    private readonly AssetService _assetService;

    public PositionSnapshotsController(
        PositionSnapshotService positionSnapshotService,
        PortfolioService portfolioService,
        AssetService assetService)
    {
        _positionSnapshotService = positionSnapshotService;
        _portfolioService = portfolioService;
        _assetService = assetService;
    }

    public async Task<IActionResult> Index(Guid? portfolioId = null, Guid? assetId = null)
    {
        ViewBag.Portfolios = await BuildPortfolioFilterListAsync(portfolioId);
        ViewBag.Assets = await BuildAssetFilterListAsync(assetId);
        ViewBag.SelectedPortfolioId = portfolioId;
        ViewBag.SelectedAssetId = assetId;

        var snapshots = await _positionSnapshotService.GetMyPositionSnapshotsAsync(portfolioId, assetId);
        return View(snapshots);
    }

    public async Task<IActionResult> Generate(Guid? portfolioId = null)
    {
        var vm = new PositionSnapshotGenerateViewModel
        {
            PortfolioId = portfolioId,
            SnapshotAt = DateTime.UtcNow
        };

        vm.PortfolioSelectList = await BuildPortfolioFilterListAsync(portfolioId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(PositionSnapshotGenerateViewModel vm)
    {
        vm.PortfolioSelectList = await BuildPortfolioFilterListAsync(vm.PortfolioId);

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var created = await _positionSnapshotService.GenerateCurrentSnapshotsAsync(vm.PortfolioId, vm.SnapshotAt);
        TempData["Info"] = created.Count == 0
            ? UiText.T("NoCurrentPositionsForSnapshotGeneration")
            : string.Format(UiText.T("GeneratedPositionSnapshots"), created.Count);

        return RedirectToAction(nameof(Index), new { portfolioId = vm.PortfolioId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid? portfolioId = null, Guid? assetId = null)
    {
        await _positionSnapshotService.DeleteAsync(id);
        return RedirectToAction(nameof(Index), new { portfolioId, assetId });
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

    private async Task<List<SelectListItem>> BuildAssetFilterListAsync(Guid? selectedValue = null)
    {
        var assets = await _assetService.GetMyAssetsAsync();
        var result = new List<SelectListItem>
        {
            new() { Value = string.Empty, Text = UiText.T("AllAssets"), Selected = selectedValue == null }
        };

        result.AddRange(assets.Select(asset => new SelectListItem
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
