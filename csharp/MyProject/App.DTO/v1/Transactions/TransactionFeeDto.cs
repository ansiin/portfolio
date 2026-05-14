namespace App.DTO.v1.Transactions;

public class TransactionFeeDto
{
    public Guid Id { get; set; }
    public string FeeType { get; set; } = default!;
    public decimal Amount { get; set; }
}
