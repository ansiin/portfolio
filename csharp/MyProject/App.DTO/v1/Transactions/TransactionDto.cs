using App.Domain.Enums;

namespace App.DTO.v1.Transactions;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid PortfolioId { get; set; }
    public string PortfolioName { get; set; } = default!;
    public Guid? AssetId { get; set; }
    public string? AssetName { get; set; }
    public TransactionType Type { get; set; }
    public DateTime ExecutedAt { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal FeeTotal { get; set; }
    public string? Description { get; set; }
    public IReadOnlyList<TransactionFeeDto> Fees { get; set; } = Array.Empty<TransactionFeeDto>();
}
