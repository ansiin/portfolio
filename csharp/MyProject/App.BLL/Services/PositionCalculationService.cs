using App.BLL.Models;
using App.DAL.EF;
using App.Domain;
using App.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class PositionCalculationService
{
    private readonly AppDbContext _context;

    public PositionCalculationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CalculatedPosition>> CalculateCurrentPositionsAsync(Guid userId, Guid? portfolioId = null)
    {
        var transactionQuery = _context.Transactions
            .Where(transaction => transaction.Portfolio!.AppUserId == userId && transaction.AssetId != null);

        if (portfolioId.HasValue)
        {
            transactionQuery = transactionQuery.Where(transaction => transaction.PortfolioId == portfolioId.Value);
        }

        var transactions = await transactionQuery
            .Include(transaction => transaction.Asset)!
            .ThenInclude(asset => asset!.Portfolio)
            .Include(transaction => transaction.Asset)!
            .ThenInclude(asset => asset!.Currency)
            .Include(transaction => transaction.Fees)
            .OrderBy(transaction => transaction.AssetId)
            .ThenBy(transaction => transaction.ExecutedAt)
            .ThenBy(transaction => transaction.Id)
            .ToListAsync();

        var snapshotQuery = _context.PriceSnapshots
            .Where(snapshot => snapshot.Asset!.Portfolio!.AppUserId == userId);

        if (portfolioId.HasValue)
        {
            snapshotQuery = snapshotQuery.Where(snapshot => snapshot.Asset!.PortfolioId == portfolioId.Value);
        }

        var latestSnapshots = await snapshotQuery
            .Include(snapshot => snapshot.Currency)
            .OrderByDescending(snapshot => snapshot.RecordedAt)
            .ThenByDescending(snapshot => snapshot.Id)
            .ToListAsync();

        var latestSnapshotByAssetId = latestSnapshots
            .GroupBy(snapshot => snapshot.AssetId)
            .ToDictionary(group => group.Key, group => group.First());

        return transactions
            .GroupBy(transaction => transaction.AssetId!.Value)
            .Select(group => MapCalculatedPosition(
                group,
                latestSnapshotByAssetId.TryGetValue(group.Key, out var latestSnapshot) ? latestSnapshot : null))
            .Where(item => item.Quantity != 0m || item.NetInvestedAmount != 0m)
            .OrderByDescending(item => item.MarketValue != 0m ? item.MarketValue : item.NetInvestedAmount)
            .ToList();
    }

    private static CalculatedPosition MapCalculatedPosition(
        IGrouping<Guid, Transaction> transactions,
        PriceSnapshot? latestSnapshot)
    {
        return PositionCalculator.Calculate(transactions, latestSnapshot);
    }
}
