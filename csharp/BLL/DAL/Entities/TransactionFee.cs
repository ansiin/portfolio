namespace DAL.Entities;

public class TransactionFee
{
    public long Id { get; set; }
    public long TransactionId { get; set; }
    public string FeeType { get; set; } = null!;
    public decimal Amount { get; set; }
    public long CurrencyId { get; set; }

    public Transaction Transaction { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
}
