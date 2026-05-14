namespace App.DTO.v1.PositionSnapshots;

public class PositionSnapshotDto
{
    public Guid Id { get; set; }
    public Guid PortfolioId { get; set; }
    public string PortfolioName { get; set; } = default!;
    public Guid? AssetId { get; set; }
    public string? AssetName { get; set; }
    public string? AssetSymbol { get; set; }
    public DateTime SnapshotAt { get; set; }
    public decimal Quantity { get; set; }
    public decimal AverageCost { get; set; }
    public decimal MarketPrice { get; set; }
    public decimal InvestedAmount { get; set; }
    public decimal MarketValue { get; set; }
    public decimal UnrealizedProfit { get; set; }
    public string? CurrencyCode { get; set; }
}
