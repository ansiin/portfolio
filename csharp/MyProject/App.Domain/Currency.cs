using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Currency : BaseEntity
{
    [StringLength(8, MinimumLength = 1)]
    public string Code { get; set; } = default!;

    [StringLength(16)]
    public string? Symbol { get; set; }

    public LangStr DisplayName { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    public ICollection<Portfolio>? Portfolios { get; set; }
    public ICollection<Asset>? Assets { get; set; }
    public ICollection<PriceSnapshot>? PriceSnapshots { get; set; }
}
