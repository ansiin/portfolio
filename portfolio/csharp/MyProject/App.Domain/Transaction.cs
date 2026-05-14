using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Enums;
using Base.Domain;

namespace App.Domain;

public class Transaction : BaseEntity
{
    public Guid PortfolioId { get; set; }
    public Portfolio? Portfolio { get; set; }

    public Guid? AssetId { get; set; }
    public Asset? Asset { get; set; }

    public TransactionType Type { get; set; }

    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "numeric(18,8)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "numeric(18,8)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "numeric(18,8)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(512)]
    public string? Description { get; set; }

    public ICollection<TransactionFee>? Fees { get; set; }
    public ICollection<Note>? Notes { get; set; }
}
