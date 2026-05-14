using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class PriceSnapshot : BaseEntity
{
    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }

    public Guid CurrencyId { get; set; }
    public Currency? Currency { get; set; }

    public Guid? MarketDataProviderId { get; set; }
    public MarketDataProvider? MarketDataProvider { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "numeric(18,8)")]
    public decimal Price { get; set; }
}
