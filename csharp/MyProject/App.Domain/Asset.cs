using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class Asset : BaseEntity
{
    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; } = default!;

    [StringLength(64)]
    public string? Symbol { get; set; }

    public Guid PortfolioId { get; set; }
    public Portfolio? Portfolio { get; set; }

    public Guid AssetTypeId { get; set; }
    public AssetType? AssetType { get; set; }

    public Guid CurrencyId { get; set; }
    public Currency? Currency { get; set; }

    public Guid? ExchangeId { get; set; }
    public Exchange? Exchange { get; set; }

    public Guid? MarketDataProviderId { get; set; }
    public MarketDataProvider? MarketDataProvider { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Transaction>? Transactions { get; set; }
    public ICollection<PriceSnapshot>? PriceSnapshots { get; set; }
    public ICollection<PositionSnapshot>? PositionSnapshots { get; set; }
    public ICollection<AssetTag>? AssetTags { get; set; }
    public ICollection<Note>? Notes { get; set; }
    public ICollection<WatchlistItem>? WatchlistItems { get; set; }
}
