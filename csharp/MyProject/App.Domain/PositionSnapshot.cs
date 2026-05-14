using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class PositionSnapshot : BaseEntity
{
    public Guid PortfolioId { get; set; }
    public Portfolio? Portfolio { get; set; }

    public Guid? AssetId { get; set; }
    public Asset? Asset { get; set; }

    public DateTime SnapshotAt { get; set; }

    [Column(TypeName = "numeric(18,8)")]
    public decimal Quantity { get; set; }

    [Column(TypeName = "numeric(18,8)")]
    public decimal AverageCost { get; set; }

    [Column(TypeName = "numeric(18,8)")]
    public decimal MarketPrice { get; set; }

    [Column(TypeName = "numeric(18,8)")]
    public decimal InvestedAmount { get; set; }

    [Column(TypeName = "numeric(18,8)")]
    public decimal MarketValue { get; set; }

    [Column(TypeName = "numeric(18,8)")]
    public decimal UnrealizedProfit { get; set; }
}
