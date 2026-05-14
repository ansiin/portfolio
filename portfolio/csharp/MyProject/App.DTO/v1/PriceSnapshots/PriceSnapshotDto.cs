namespace App.DTO.v1.PriceSnapshots;

public class PriceSnapshotDto
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = default!;
    public string? AssetSymbol { get; set; }
    public string PortfolioName { get; set; } = default!;
    public Guid CurrencyId { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public Guid? MarketDataProviderId { get; set; }
    public string? MarketDataProviderCode { get; set; }
    public DateTime RecordedAt { get; set; }
    public decimal Price { get; set; }
}
