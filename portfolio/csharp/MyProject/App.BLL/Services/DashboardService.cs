using App.BLL.Abstractions;
using App.DAL.EF;
using App.Domain.Enums;
using App.DTO.v1.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class DashboardService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly PositionCalculationService _positionCalculationService;

    public DashboardService(
        AppDbContext context,
        ICurrentUserService currentUserService,
        PositionCalculationService positionCalculationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _positionCalculationService = positionCalculationService;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var userId = _currentUserService.GetRequiredUserId();
        var positions = await _positionCalculationService.CalculateCurrentPositionsAsync(userId);
        var portfolioCount = await _context.Portfolios
            .Where(portfolio => portfolio.AppUserId == userId)
            .CountAsync();

        var activeAssetCount = await _context.Assets
            .Where(asset => asset.Portfolio!.AppUserId == userId && asset.IsActive)
            .CountAsync();

        var transactions = await _context.Transactions
            .Where(transaction => transaction.Portfolio!.AppUserId == userId)
            .Select(transaction => new
            {
                transaction.Type,
                transaction.TotalAmount
            })
            .ToListAsync();

        return new DashboardSummaryDto
        {
            PortfolioCount = portfolioCount,
            ActiveAssetCount = activeAssetCount,
            TransactionCount = transactions.Count,
            NetCashFlow = transactions.Sum(transaction =>
                transaction.Type == TransactionType.Deposit ? transaction.TotalAmount :
                transaction.Type == TransactionType.Withdrawal ? -transaction.TotalAmount :
                transaction.Type == TransactionType.Dividend ? transaction.TotalAmount : 0m),
            BuyVolume = transactions
                .Where(transaction => transaction.Type == TransactionType.Buy)
                .Sum(transaction => transaction.TotalAmount),
            SellVolume = transactions
                .Where(transaction => transaction.Type == TransactionType.Sell)
                .Sum(transaction => transaction.TotalAmount),
            TotalMarketValue = positions.Sum(item => item.MarketValue),
            TotalUnrealizedProfit = positions.Sum(item => item.UnrealizedProfit)
        };
    }

    public async Task<IReadOnlyList<DashboardAllocationItemDto>> GetAllocationAsync()
    {
        var userId = _currentUserService.GetRequiredUserId();
        var positions = await _positionCalculationService.CalculateCurrentPositionsAsync(userId);

        return positions.Select(position => new DashboardAllocationItemDto
        {
            AssetId = position.AssetId,
            AssetName = position.AssetName,
            AssetSymbol = position.AssetSymbol,
            PortfolioName = position.PortfolioName,
            Quantity = position.Quantity,
            NetInvestedAmount = position.NetInvestedAmount,
            CostBasisAmount = position.CostBasisAmount,
            AverageCost = position.AverageCost,
            LatestPrice = position.LatestPrice,
            LatestPriceRecordedAt = position.LatestPriceRecordedAt,
            ValuationCurrencyCode = position.ValuationCurrencyCode,
            MarketValue = position.MarketValue,
            UnrealizedProfit = position.UnrealizedProfit
        }).ToList();
    }

    public async Task<IReadOnlyList<DashboardTimelinePointDto>> GetTimelineAsync()
    {
        var userId = _currentUserService.GetRequiredUserId();

        var items = await _context.Transactions
            .Where(transaction => transaction.Portfolio!.AppUserId == userId)
            .Select(transaction => new
            {
                transaction.ExecutedAt,
                transaction.Type,
                transaction.TotalAmount
            })
            .ToListAsync();

        return items
            .GroupBy(transaction => new { transaction.ExecutedAt.Year, transaction.ExecutedAt.Month })
            .Select(group => new DashboardTimelinePointDto
            {
                Period = $"{group.Key.Year:D4}-{group.Key.Month:D2}",
                NetAmount = group.Sum(transaction =>
                    transaction.Type == TransactionType.Deposit ? transaction.TotalAmount :
                    transaction.Type == TransactionType.Withdrawal ? -transaction.TotalAmount :
                    transaction.Type == TransactionType.Buy ? -transaction.TotalAmount :
                    transaction.Type == TransactionType.Sell ? transaction.TotalAmount :
                    transaction.Type == TransactionType.Dividend ? transaction.TotalAmount : 0m)
            })
            .OrderBy(item => item.Period)
            .ToList();
    }
}
