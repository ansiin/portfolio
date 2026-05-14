namespace App.DTO.v1.Dashboard;

public class DashboardAllocationItemDto
{
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = default!;
    public string? AssetSymbol { get; set; }
    public string PortfolioName { get; set; } = default!;
    public decimal Quantity { get; set; }
    public decimal NetInvestedAmount { get; set; }
    public decimal CostBasisAmount { get; set; }
    public decimal AverageCost { get; set; }
    public decimal? LatestPrice { get; set; }
    public DateTime? LatestPriceRecordedAt { get; set; }
    public string? ValuationCurrencyCode { get; set; }
    public decimal MarketValue { get; set; }
    public decimal UnrealizedProfit { get; set; }
}
