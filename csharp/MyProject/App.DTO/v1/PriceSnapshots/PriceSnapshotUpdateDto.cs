namespace App.DTO.v1.PriceSnapshots;

public class PriceSnapshotUpdateDto
{
    public Guid AssetId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? MarketDataProviderId { get; set; }
    public DateTime RecordedAt { get; set; }
    public decimal Price { get; set; }
}
