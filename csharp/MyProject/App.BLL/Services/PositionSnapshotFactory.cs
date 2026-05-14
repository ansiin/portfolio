using App.BLL.Models;
using App.Domain;

namespace App.BLL.Services;

public static class PositionSnapshotFactory
{
    public static PositionSnapshot Create(CalculatedPosition position, DateTime snapshotAt)
    {
        return new PositionSnapshot
        {
            PortfolioId = position.PortfolioId,
            AssetId = position.AssetId,
            SnapshotAt = snapshotAt,
            Quantity = position.Quantity,
            AverageCost = position.AverageCost,
            MarketPrice = position.LatestPrice ?? 0m,
            InvestedAmount = position.CostBasisAmount,
            MarketValue = position.MarketValue,
            UnrealizedProfit = position.UnrealizedProfit
        };
    }
}
