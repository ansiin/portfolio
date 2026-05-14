using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1.Transactions;

public class TransactionFeeInputDto
{
    [StringLength(64, MinimumLength = 1)]
    public string FeeType { get; set; } = default!;

    public decimal Amount { get; set; }
}
