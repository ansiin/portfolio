namespace App.DTO.v1.Dashboard;

public class DashboardSummaryDto
{
    public int PortfolioCount { get; set; }
    public int ActiveAssetCount { get; set; }
    public int TransactionCount { get; set; }
    public decimal NetCashFlow { get; set; }
    public decimal BuyVolume { get; set; }
    public decimal SellVolume { get; set; }
    public decimal TotalMarketValue { get; set; }
    public decimal TotalUnrealizedProfit { get; set; }
}
