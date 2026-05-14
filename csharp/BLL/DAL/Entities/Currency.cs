namespace DAL.Entities;

public class Currency
{
    public long Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Symbol { get; set; }

    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
    public ICollection<PositionSnapshot> PositionCostSnapshots { get; set; } = new List<PositionSnapshot>();
    public ICollection<PositionSnapshot> PositionMarketSnapshots { get; set; } = new List<PositionSnapshot>();
    public ICollection<PriceSnapshot> PriceSnapshots { get; set; } = new List<PriceSnapshot>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<TransactionFee> TransactionFees { get; set; } = new List<TransactionFee>();
}
