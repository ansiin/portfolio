using App.BLL.Models;
using App.Domain;
using App.Domain.Enums;

namespace App.BLL.Services;

public static class PositionCalculator
{
    public static CalculatedPosition Calculate(
        IEnumerable<Transaction> transactions,
        PriceSnapshot? latestSnapshot)
    {
        var orderedTransactions = transactions
            .OrderBy(transaction => transaction.ExecutedAt)
            .ThenBy(transaction => transaction.Id)
            .ToList();

        if (orderedTransactions.Count == 0)
        {
            throw new ArgumentException("At least one transaction is required for position calculation.", nameof(transactions));
        }

        var firstTransaction = orderedTransactions[0];
        var asset = firstTransaction.Asset ?? throw new InvalidOperationException("Asset is required for position calculation.");
        var portfolio = asset.Portfolio ?? throw new InvalidOperationException("Portfolio is required for position calculation.");

        decimal quantity = 0m;
        decimal netInvestedAmount = 0m;
        decimal costBasisAmount = 0m;

        foreach (var transaction in orderedTransactions)
        {
            var feeTotal = transaction.Fees?.Sum(fee => fee.Amount) ?? 0m;

            switch (transaction.Type)
            {
                case TransactionType.Buy:
                    quantity += transaction.Quantity;
                    costBasisAmount += transaction.TotalAmount + feeTotal;
                    netInvestedAmount += transaction.TotalAmount + feeTotal;
                    break;
                case TransactionType.Sell:
                {
                    netInvestedAmount += -transaction.TotalAmount + feeTotal;

                    if (quantity <= 0m)
                    {
                        break;
                    }

                    var averageCost = costBasisAmount / quantity;
                    var soldQuantity = transaction.Quantity >= quantity ? quantity : transaction.Quantity;
                    costBasisAmount -= averageCost * soldQuantity;
                    quantity -= soldQuantity;

                    if (quantity == 0m)
                    {
                        costBasisAmount = 0m;
                    }

                    break;
                }
                case TransactionType.Dividend:
                    netInvestedAmount -= transaction.TotalAmount;
                    break;
            }
        }

        var latestPrice = latestSnapshot?.Price;
        var marketValue = latestPrice.HasValue ? quantity * latestPrice.Value : 0m;
        var unrealizedProfit = latestPrice.HasValue ? marketValue - costBasisAmount : 0m;
        var averageCostCurrent = quantity > 0m ? costBasisAmount / quantity : 0m;

        return new CalculatedPosition
        {
            PortfolioId = portfolio.Id,
            PortfolioName = portfolio.Name,
            AssetId = asset.Id,
            AssetName = asset.Name,
            AssetSymbol = asset.Symbol,
            Quantity = quantity,
            NetInvestedAmount = netInvestedAmount,
            CostBasisAmount = costBasisAmount,
            AverageCost = averageCostCurrent,
            LatestPrice = latestPrice,
            LatestPriceRecordedAt = latestSnapshot?.RecordedAt,
            ValuationCurrencyCode = latestSnapshot?.Currency?.Code ?? asset.Currency?.Code,
            MarketValue = marketValue,
            UnrealizedProfit = unrealizedProfit
        };
    }
}
