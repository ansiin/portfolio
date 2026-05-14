using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class TransactionFee : BaseEntity
{
    public Guid TransactionId { get; set; }
    public Transaction? Transaction { get; set; }

    [StringLength(64)]
    public string FeeType { get; set; } = default!;

    [Column(TypeName = "numeric(18,8)")]
    public decimal Amount { get; set; }
}
