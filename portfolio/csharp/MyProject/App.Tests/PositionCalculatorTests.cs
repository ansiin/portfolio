using App.BLL.Services;
using App.Domain;
using App.Domain.Enums;
using Base.Domain;

namespace App.Tests;

public class PositionCalculatorTests
{
    [Fact]
    public void Calculate_HandlesBuysSellDividendAndLatestPrice()
    {
        var currency = new Currency
        {
            Id = Guid.NewGuid(),
            Code = "USD",
            DisplayName = new LangStr("US Dollar", "en")
        };

        var portfolio = new Portfolio
        {
            Id = Guid.NewGuid(),
            Name = "Main Portfolio",
            AppUserId = Guid.NewGuid(),
            BaseCurrencyId = currency.Id,
            BaseCurrency = currency
        };

        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            Name = "Apple Inc.",
            Symbol = "AAPL",
            PortfolioId = portfolio.Id,
            Portfolio = portfolio,
            CurrencyId = currency.Id,
            Currency = currency
        };

        var buy1 = new Transaction
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            AssetId = asset.Id,
            Asset = asset,
            PortfolioId = portfolio.Id,
            Portfolio = portfolio,
            Type = TransactionType.Buy,
            ExecutedAt = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc),
            Quantity = 10m,
            UnitPrice = 100m,
            TotalAmount = 1000m,
            Fees = new List<TransactionFee>
            {
                new() { FeeType = "Broker", Amount = 5m }
            }
        };

        var buy2 = new Transaction
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            AssetId = asset.Id,
            Asset = asset,
            PortfolioId = portfolio.Id,
            Portfolio = portfolio,
            Type = TransactionType.Buy,
            ExecutedAt = new DateTime(2026, 1, 5, 10, 0, 0, DateTimeKind.Utc),
            Quantity = 5m,
            UnitPrice = 120m,
            TotalAmount = 600m,
            Fees = new List<TransactionFee>
            {
                new() { FeeType = "Broker", Amount = 3m }
            }
        };

        var dividend = new Transaction
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            AssetId = asset.Id,
            Asset = asset,
            PortfolioId = portfolio.Id,
            Portfolio = portfolio,
            Type = TransactionType.Dividend,
            ExecutedAt = new DateTime(2026, 1, 10, 10, 0, 0, DateTimeKind.Utc),
            TotalAmount = 50m
        };

        var sell = new Transaction
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
            AssetId = asset.Id,
            Asset = asset,
            PortfolioId = portfolio.Id,
            Portfolio = portfolio,
            Type = TransactionType.Sell,
            ExecutedAt = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc),
            Quantity = 8m,
            UnitPrice = 150m,
            TotalAmount = 1200m,
            Fees = new List<TransactionFee>
            {
                new() { FeeType = "Broker", Amount = 2m }
            }
        };

        var latestSnapshot = new PriceSnapshot
        {
            Id = Guid.NewGuid(),
            AssetId = asset.Id,
            Asset = asset,
            CurrencyId = currency.Id,
            Currency = currency,
            RecordedAt = new DateTime(2026, 2, 1, 12, 0, 0, DateTimeKind.Utc),
            Price = 130m
        };

        var result = PositionCalculator.Calculate(new[] { sell, buy2, dividend, buy1 }, latestSnapshot);

        Assert.Equal(portfolio.Id, result.PortfolioId);
        Assert.Equal(asset.Id, result.AssetId);
        Assert.Equal("Main Portfolio", result.PortfolioName);
        Assert.Equal("Apple Inc.", result.AssetName);
        Assert.Equal("AAPL", result.AssetSymbol);
        Assert.Equal(7m, result.Quantity);
        Assert.Equal(360m, result.NetInvestedAmount);
        Assert.Equal(750.4m, result.CostBasisAmount);
        Assert.Equal(107.2m, result.AverageCost);
        Assert.Equal(130m, result.LatestPrice);
        Assert.Equal(new DateTime(2026, 2, 1, 12, 0, 0, DateTimeKind.Utc), result.LatestPriceRecordedAt);
        Assert.Equal("USD", result.ValuationCurrencyCode);
        Assert.Equal(910m, result.MarketValue);
        Assert.Equal(159.6m, result.UnrealizedProfit);
    }
}
