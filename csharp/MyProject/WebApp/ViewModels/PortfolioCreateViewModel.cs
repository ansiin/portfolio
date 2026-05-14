using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class PortfolioCreateViewModel
{
    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; } = default!;

    public Guid BaseCurrencyId { get; set; }

    [ValidateNever]
    public List<SelectListItem> CurrencySelectList { get; set; } = new();
}
