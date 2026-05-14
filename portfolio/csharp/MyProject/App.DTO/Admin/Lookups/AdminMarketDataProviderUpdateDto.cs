using System.ComponentModel.DataAnnotations;

namespace App.DTO.Admin.Lookups;

public class AdminMarketDataProviderUpdateDto
{
    [StringLength(32, MinimumLength = 1)]
    public string Code { get; set; } = default!;

    [StringLength(128, MinimumLength = 1)]
    public string DisplayNameEn { get; set; } = default!;

    [StringLength(128, MinimumLength = 1)]
    public string DisplayNameEt { get; set; } = default!;

    [StringLength(256)]
    public string? BaseUrl { get; set; }

    public bool IsActive { get; set; }
}
