namespace DAL.Entities;

public class PositionSnapshot
{
    public long Id { get; set; }
    public long PortfolioId { get; set; }
    public long AssetId { get; set; }
    public DateOnly Month { get; set; }
    public decimal Quantity { get; set; }
    public decimal AvgCost { get; set; }
    public long CostCurrencyId { get; set; }
    public decimal? MarketPrice { get; set; }
    public long? MarketCurrencyId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Portfolio Portfolio { get; set; } = null!;
    public Asset Asset { get; set; } = null!;
    public Currency CostCurrency { get; set; } = null!;
    public Currency? MarketCurrency { get; set; }
}
