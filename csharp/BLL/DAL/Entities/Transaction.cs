namespace DAL.Entities;

public class Transaction
{
    public long Id { get; set; }
    public long PortfolioId { get; set; }
    public long? AssetId { get; set; }
    public DateTimeOffset TradeTime { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public long CurrencyId { get; set; }
    public decimal? CashAmount { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Portfolio Portfolio { get; set; } = null!;
    public Asset? Asset { get; set; }
    public Currency Currency { get; set; } = null!;
    public ICollection<TransactionFee> TransactionFees { get; set; } = new List<TransactionFee>();
    public ICollection<Note> Notes { get; set; } = new List<Note>();
}
