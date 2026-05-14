namespace DAL.Entities;

public class Portfolio
{
    public long Id { get; set; }
    public string AppUserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public long BaseCurrencyId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Currency BaseCurrency { get; set; } = null!;
    public ICollection<PositionSnapshot> PositionSnapshots { get; set; } = new List<PositionSnapshot>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
