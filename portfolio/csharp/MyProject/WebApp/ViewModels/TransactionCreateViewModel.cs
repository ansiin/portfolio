using System.ComponentModel.DataAnnotations;
using App.Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class TransactionCreateViewModel
{
    public Guid PortfolioId { get; set; }
    public Guid? AssetId { get; set; }
    public TransactionType Type { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

    [StringLength(64)]
    public string? FeeType { get; set; }

    public decimal? FeeAmount { get; set; }

    [ValidateNever]
    public List<SelectListItem> PortfolioSelectList { get; set; } = new();

    [ValidateNever]
    public List<SelectListItem> AssetSelectList { get; set; } = new();

    [ValidateNever]
    public List<SelectListItem> TransactionTypeSelectList { get; set; } = new();
}
