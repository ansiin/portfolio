using System.ComponentModel.DataAnnotations;
using App.Domain.Enums;

namespace App.DTO.v1.Transactions;

public class TransactionUpdateDto
{
    public Guid PortfolioId { get; set; }
    public Guid? AssetId { get; set; }
    public TransactionType Type { get; set; }
    public DateTime ExecutedAt { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

    public List<TransactionFeeInputDto> Fees { get; set; } = new();
}
