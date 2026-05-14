using System.ComponentModel.DataAnnotations;
using App.DTO.v1.Watchlists;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class WatchlistEditViewModel
{
    public Guid Id { get; set; }

    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; } = default!;

    public Guid? AssetId { get; set; }

    [ValidateNever]
    public List<SelectListItem> AssetSelectList { get; set; } = new();

    [ValidateNever]
    public IReadOnlyList<WatchlistItemDto> Items { get; set; } = Array.Empty<WatchlistItemDto>();
}
