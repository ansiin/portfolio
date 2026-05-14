namespace DAL.Entities;

public class PriceSnapshot
{
    public long Id { get; set; }
    public long AssetId { get; set; }
    public long ProviderId { get; set; }
    public decimal Price { get; set; }
    public long CurrencyId { get; set; }
    public DateTimeOffset AsOf { get; set; }

    public Asset Asset { get; set; } = null!;
    public MarketDataProvider Provider { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
}
