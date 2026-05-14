using System.ComponentModel.DataAnnotations;

namespace WebApp.Areas.Admin.ViewModels;

public class CurrencyEditViewModel
{
    public Guid Id { get; set; }

    [StringLength(8, MinimumLength = 1)]
    public string Code { get; set; } = default!;

    [StringLength(16)]
    public string? Symbol { get; set; }

    [StringLength(128, MinimumLength = 1)]
    public string DisplayNameEn { get; set; } = default!;

    [StringLength(128, MinimumLength = 1)]
    public string DisplayNameEt { get; set; } = default!;

    public bool IsActive { get; set; } = true;
}
