using App.BLL.Models;
using App.BLL.Services;

namespace App.Tests;

public class PositionSnapshotFactoryTests
{
    [Fact]
    public void Create_MapsCalculatedPositionIntoPersistedSnapshotShape()
    {
        var portfolioId = Guid.NewGuid();
        var assetId = Guid.NewGuid();
        var snapshotAt = new DateTime(2026, 3, 1, 8, 30, 0, DateTimeKind.Utc);

        var position = new CalculatedPosition
        {
            PortfolioId = portfolioId,
            PortfolioName = "Growth",
            AssetId = assetId,
            AssetName = "NVIDIA",
            AssetSymbol = "NVDA",
            Quantity = 12m,
            NetInvestedAmount = 900m,
            CostBasisAmount = 1020m,
            AverageCost = 85m,
            LatestPrice = 95m,
            LatestPriceRecordedAt = new DateTime(2026, 2, 28, 0, 0, 0, DateTimeKind.Utc),
            ValuationCurrencyCode = "USD",
            MarketValue = 1140m,
            UnrealizedProfit = 120m
        };

        var snapshot = PositionSnapshotFactory.Create(position, snapshotAt);

        Assert.Equal(portfolioId, snapshot.PortfolioId);
        Assert.Equal(assetId, snapshot.AssetId);
        Assert.Equal(snapshotAt, snapshot.SnapshotAt);
        Assert.Equal(12m, snapshot.Quantity);
        Assert.Equal(85m, snapshot.AverageCost);
        Assert.Equal(95m, snapshot.MarketPrice);
        Assert.Equal(1020m, snapshot.InvestedAmount);
        Assert.Equal(1140m, snapshot.MarketValue);
        Assert.Equal(120m, snapshot.UnrealizedProfit);
    }

    [Fact]
    public void Create_UsesZeroMarketPriceWhenLatestPriceIsMissing()
    {
        var position = new CalculatedPosition
        {
            PortfolioId = Guid.NewGuid(),
            PortfolioName = "Income",
            AssetId = Guid.NewGuid(),
            AssetName = "Bond ETF",
            Quantity = 4m,
            CostBasisAmount = 400m,
            AverageCost = 100m,
            MarketValue = 0m,
            UnrealizedProfit = 0m
        };

        var snapshot = PositionSnapshotFactory.Create(position, new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc));

        Assert.Equal(0m, snapshot.MarketPrice);
    }
}
