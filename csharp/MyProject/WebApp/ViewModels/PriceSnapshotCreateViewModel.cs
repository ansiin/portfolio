using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class PriceSnapshotCreateViewModel
{
    public Guid AssetId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? MarketDataProviderId { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public decimal Price { get; set; }

    [ValidateNever]
    public List<SelectListItem> AssetSelectList { get; set; } = new();

    [ValidateNever]
    public List<SelectListItem> CurrencySelectList { get; set; } = new();

    [ValidateNever]
    public List<SelectListItem> ProviderSelectList { get; set; } = new();
}
