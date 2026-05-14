using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class AssetEditViewModel
{
    public Guid Id { get; set; }
    public Guid PortfolioId { get; set; }

    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; } = default!;

    [StringLength(64)]
    public string? Symbol { get; set; }

    public Guid AssetTypeId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? ExchangeId { get; set; }
    public Guid? MarketDataProviderId { get; set; }
    public bool IsActive { get; set; }

    [ValidateNever]
    public List<SelectListItem> AssetTypeSelectList { get; set; } = new();

    [ValidateNever]
    public List<SelectListItem> CurrencySelectList { get; set; } = new();

    [ValidateNever]
    public List<SelectListItem> ExchangeSelectList { get; set; } = new();

    [ValidateNever]
    public List<SelectListItem> ProviderSelectList { get; set; } = new();
}
