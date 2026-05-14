using App.BLL.Services;
using App.Domain.Enums;
using App.DTO.v1.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Helpers;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly TransactionService _transactionService;
    private readonly PortfolioService _portfolioService;
    private readonly AssetService _assetService;

    public TransactionsController(
        TransactionService transactionService,
        PortfolioService portfolioService,
        AssetService assetService)
    {
        _transactionService = transactionService;
        _portfolioService = portfolioService;
        _assetService = assetService;
    }

    public async Task<IActionResult> Index(Guid? portfolioId = null)
    {
        ViewBag.Portfolios = await BuildPortfolioFilterListAsync(portfolioId);
        ViewBag.SelectedPortfolioId = portfolioId;

        var transactions = await _transactionService.GetMyTransactionsAsync(portfolioId);
        return View(transactions);
    }

    public async Task<IActionResult> Create(Guid? portfolioId = null)
    {
        var vm = new TransactionCreateViewModel
        {
            PortfolioId = portfolioId ?? Guid.Empty,
            ExecutedAt = DateTime.UtcNow
        };

        await PopulateCreateLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransactionCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCreateLookupsAsync(vm);
            return View(vm);
        }

        try
        {
            await _transactionService.CreateAsync(new TransactionCreateDto
            {
                PortfolioId = vm.PortfolioId,
                AssetId = vm.AssetId,
                Type = vm.Type,
                ExecutedAt = vm.ExecutedAt,
                Quantity = vm.Quantity,
                UnitPrice = vm.UnitPrice,
                TotalAmount = vm.TotalAmount,
                Description = vm.Description,
                Fees = BuildFees(vm.FeeType, vm.FeeAmount)
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
        var transaction = await _transactionService.GetMyTransactionAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }

        var primaryFee = transaction.Fees.FirstOrDefault();
        var vm = new TransactionEditViewModel
        {
            Id = transaction.Id,
            PortfolioId = transaction.PortfolioId,
            AssetId = transaction.AssetId,
            Type = transaction.Type,
            ExecutedAt = transaction.ExecutedAt,
            Quantity = transaction.Quantity,
            UnitPrice = transaction.UnitPrice,
            TotalAmount = transaction.TotalAmount,
            Description = transaction.Description,
            FeeType = primaryFee?.FeeType,
            FeeAmount = primaryFee?.Amount
        };

        await PopulateEditLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, TransactionEditViewModel vm)
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
            var updated = await _transactionService.UpdateAsync(id, new TransactionUpdateDto
            {
                PortfolioId = vm.PortfolioId,
                AssetId = vm.AssetId,
                Type = vm.Type,
                ExecutedAt = vm.ExecutedAt,
                Quantity = vm.Quantity,
                UnitPrice = vm.UnitPrice,
                TotalAmount = vm.TotalAmount,
                Description = vm.Description,
                Fees = BuildFees(vm.FeeType, vm.FeeAmount)
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
        await _transactionService.DeleteAsync(id);
        return RedirectToAction(nameof(Index), new { portfolioId });
    }

    private async Task PopulateCreateLookupsAsync(TransactionCreateViewModel vm)
    {
        vm.PortfolioSelectList = await BuildPortfolioSelectListAsync(vm.PortfolioId);
        vm.AssetSelectList = await BuildAssetSelectListAsync(vm.PortfolioId, vm.AssetId);
        vm.TransactionTypeSelectList = BuildTransactionTypeSelectList(vm.Type);
    }

    private async Task PopulateEditLookupsAsync(TransactionEditViewModel vm)
    {
        vm.PortfolioSelectList = await BuildPortfolioSelectListAsync(vm.PortfolioId);
        vm.AssetSelectList = await BuildAssetSelectListAsync(vm.PortfolioId, vm.AssetId);
        vm.TransactionTypeSelectList = BuildTransactionTypeSelectList(vm.Type);
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

    private async Task<List<SelectListItem>> BuildAssetSelectListAsync(Guid? portfolioId, Guid? selectedValue = null)
    {
        var normalizedPortfolioId = portfolioId == Guid.Empty ? null : portfolioId;
        var assets = await _assetService.GetMyAssetsAsync(normalizedPortfolioId);

        var result = new List<SelectListItem>
        {
            new() { Value = string.Empty, Text = UiText.T("NoAsset"), Selected = selectedValue == null }
        };

        result.AddRange(assets
            .Where(asset => asset.IsActive || asset.Id == selectedValue)
            .Select(asset => new SelectListItem
            {
                Value = asset.Id.ToString(),
                Text = asset.Symbol is { Length: > 0 } ? $"{asset.Name} ({asset.Symbol})" : asset.Name,
                Selected = selectedValue == asset.Id
            }));

        return result;
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

    private static List<SelectListItem> BuildTransactionTypeSelectList(TransactionType selectedValue)
    {
        return Enum.GetValues<TransactionType>()
            .Select(type => new SelectListItem
            {
                Value = type.ToString(),
                Text = UiText.T($"TransactionType_{type}"),
                Selected = type == selectedValue
            })
            .ToList();
    }

    private static List<TransactionFeeInputDto> BuildFees(string? feeType, decimal? feeAmount)
    {
        if (string.IsNullOrWhiteSpace(feeType) || feeAmount is null or 0)
        {
            return new List<TransactionFeeInputDto>();
        }

        return new List<TransactionFeeInputDto>
        {
            new TransactionFeeInputDto
            {
                FeeType = feeType.Trim(),
                Amount = feeAmount.Value
            }
        };
    }
}
