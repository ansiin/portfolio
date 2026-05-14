namespace DAL.Entities;

public class Asset
{
    public long Id { get; set; }
    public long AssetTypeId { get; set; }
    public long? ExchangeId { get; set; }
    public long CurrencyId { get; set; }
    public string? Symbol { get; set; }
    public string Name { get; set; } = null!;
    public string? SteamMarketHashName { get; set; }
    public string? ExternalId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }

    public AssetType AssetType { get; set; } = null!;
    public Exchange? Exchange { get; set; }
    public Currency Currency { get; set; } = null!;
    public ICollection<CorporateAction> CorporateActions { get; set; } = new List<CorporateAction>();
    public ICollection<PositionSnapshot> PositionSnapshots { get; set; } = new List<PositionSnapshot>();
    public ICollection<PriceSnapshot> PriceSnapshots { get; set; } = new List<PriceSnapshot>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<AssetTag> AssetTags { get; set; } = new List<AssetTag>();
    public ICollection<Note> Notes { get; set; } = new List<Note>();
    public ICollection<WatchlistItem> WatchlistItems { get; set; } = new List<WatchlistItem>();
}
