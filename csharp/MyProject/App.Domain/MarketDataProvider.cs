using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class MarketDataProvider : BaseEntity
{
    [StringLength(32, MinimumLength = 1)]
    public string Code { get; set; } = default!;

    public LangStr DisplayName { get; set; } = default!;

    [StringLength(256)]
    public string? BaseUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Asset>? Assets { get; set; }
    public ICollection<PriceSnapshot>? PriceSnapshots { get; set; }
}
