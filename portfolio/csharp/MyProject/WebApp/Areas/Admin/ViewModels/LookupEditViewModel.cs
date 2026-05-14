using System.ComponentModel.DataAnnotations;

namespace WebApp.Areas.Admin.ViewModels;

public class LookupEditViewModel
{
    public Guid Id { get; set; }

    [StringLength(32, MinimumLength = 1)]
    public string Code { get; set; } = default!;

    [StringLength(128, MinimumLength = 1)]
    public string DisplayNameEn { get; set; } = default!;

    [StringLength(128, MinimumLength = 1)]
    public string DisplayNameEt { get; set; } = default!;

    public bool IsActive { get; set; } = true;
}
