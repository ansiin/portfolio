namespace App.DTO.v1.PriceSnapshots;

public class PriceSnapshotCreateDto
{
    public Guid AssetId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? MarketDataProviderId { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public decimal Price { get; set; }
}
