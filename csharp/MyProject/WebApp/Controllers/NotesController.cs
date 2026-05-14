using App.BLL.Services;
using App.DTO.v1.Notes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Helpers;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class NotesController : Controller
{
    private readonly NoteService _noteService;
    private readonly AssetService _assetService;
    private readonly TransactionService _transactionService;

    public NotesController(NoteService noteService, AssetService assetService, TransactionService transactionService)
    {
        _noteService = noteService;
        _assetService = assetService;
        _transactionService = transactionService;
    }

    public async Task<IActionResult> Index(Guid? assetId = null, Guid? transactionId = null)
    {
        var notes = await _noteService.GetMyNotesAsync(assetId, transactionId);
        return View(notes);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new NoteCreateViewModel();
        await PopulateCreateLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NoteCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCreateLookupsAsync(vm);
            return View(vm);
        }

        try
        {
            await _noteService.CreateAsync(new NoteCreateDto
            {
                Title = vm.Title,
                Content = vm.Content,
                AssetId = vm.AssetId,
                TransactionId = vm.TransactionId
            });

            return RedirectToAction(nameof(Index));
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
        var note = await _noteService.GetMyNoteAsync(id);
        if (note == null)
        {
            return NotFound();
        }

        var vm = new NoteEditViewModel
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            AssetId = note.AssetId,
            TransactionId = note.TransactionId
        };

        await PopulateEditLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, NoteEditViewModel vm)
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
            var updated = await _noteService.UpdateAsync(id, new NoteUpdateDto
            {
                Title = vm.Title,
                Content = vm.Content,
                AssetId = vm.AssetId,
                TransactionId = vm.TransactionId
            });

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
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
    public async Task<IActionResult> Delete(Guid id)
    {
        await _noteService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCreateLookupsAsync(NoteCreateViewModel vm)
    {
        vm.AssetSelectList = await BuildAssetSelectListAsync(vm.AssetId);
        vm.TransactionSelectList = await BuildTransactionSelectListAsync(vm.TransactionId);
    }

    private async Task PopulateEditLookupsAsync(NoteEditViewModel vm)
    {
        vm.AssetSelectList = await BuildAssetSelectListAsync(vm.AssetId);
        vm.TransactionSelectList = await BuildTransactionSelectListAsync(vm.TransactionId);
    }

    private async Task<List<SelectListItem>> BuildAssetSelectListAsync(Guid? selectedValue = null)
    {
        var assets = await _assetService.GetMyAssetsAsync();
        var result = new List<SelectListItem>
        {
            new() { Value = string.Empty, Text = UiText.T("NoAsset"), Selected = selectedValue == null }
        };

        result.AddRange(assets
            .Where(asset => asset.IsActive || asset.Id == selectedValue)
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

    private async Task<List<SelectListItem>> BuildTransactionSelectListAsync(Guid? selectedValue = null)
    {
        var transactions = await _transactionService.GetMyTransactionsAsync();
        var result = new List<SelectListItem>
        {
            new() { Value = string.Empty, Text = UiText.T("NoTransaction"), Selected = selectedValue == null }
        };

        result.AddRange(transactions
            .OrderByDescending(transaction => transaction.ExecutedAt)
            .Select(transaction => new SelectListItem
            {
                Value = transaction.Id.ToString(),
                Text = transaction.AssetName is { Length: > 0 }
                    ? $"{UiText.T($"TransactionType_{transaction.Type}")} {transaction.ExecutedAt:yyyy-MM-dd} ({transaction.AssetName})"
                    : $"{UiText.T($"TransactionType_{transaction.Type}")} {transaction.ExecutedAt:yyyy-MM-dd}",
                Selected = selectedValue == transaction.Id
            }));

        return result;
    }
}
