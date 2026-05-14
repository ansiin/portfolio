using App.BLL.Abstractions;
using App.DAL.EF;
using App.Domain;
using App.DTO.v1.PositionSnapshots;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class PositionSnapshotService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly PositionCalculationService _positionCalculationService;

    public PositionSnapshotService(
        AppDbContext context,
        ICurrentUserService currentUserService,
        PositionCalculationService positionCalculationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _positionCalculationService = positionCalculationService;
    }

    public async Task<IReadOnlyList<PositionSnapshotDto>> GetMyPositionSnapshotsAsync(Guid? portfolioId = null, Guid? assetId = null)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var query = BuildQuery(userId);

        if (portfolioId.HasValue)
        {
            query = query.Where(snapshot => snapshot.PortfolioId == portfolioId.Value);
        }

        if (assetId.HasValue)
        {
            query = query.Where(snapshot => snapshot.AssetId == assetId.Value);
        }

        return await query
            .OrderByDescending(snapshot => snapshot.SnapshotAt)
            .ThenBy(snapshot => snapshot.PortfolioName)
            .ThenBy(snapshot => snapshot.AssetName)
            .ToListAsync();
    }

    public async Task<PositionSnapshotDto?> GetMyPositionSnapshotAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        return await BuildQuery(userId)
            .FirstOrDefaultAsync(snapshot => snapshot.Id == id);
    }

    public async Task<IReadOnlyList<PositionSnapshotDto>> GenerateCurrentSnapshotsAsync(Guid? portfolioId = null, DateTime? snapshotAt = null)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var effectiveSnapshotAt = snapshotAt ?? DateTime.UtcNow;
        var positions = await _positionCalculationService.CalculateCurrentPositionsAsync(userId, portfolioId);

        if (positions.Count == 0)
        {
            return Array.Empty<PositionSnapshotDto>();
        }

        var entities = positions
            .Select(position => PositionSnapshotFactory.Create(position, effectiveSnapshotAt))
            .ToList();

        _context.PositionSnapshots.AddRange(entities);
        await _context.SaveChangesAsync();

        var createdIds = entities.Select(entity => entity.Id).ToHashSet();

        return await BuildQuery(userId)
            .Where(snapshot => createdIds.Contains(snapshot.Id))
            .OrderByDescending(snapshot => snapshot.SnapshotAt)
            .ThenBy(snapshot => snapshot.PortfolioName)
            .ThenBy(snapshot => snapshot.AssetName)
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.PositionSnapshots
            .Include(snapshot => snapshot.Portfolio)
            .AsTracking()
            .FirstOrDefaultAsync(snapshot => snapshot.Id == id && snapshot.Portfolio!.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        _context.PositionSnapshots.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<PositionSnapshotDto> BuildQuery(Guid userId)
    {
        return _context.PositionSnapshots
            .Where(snapshot => snapshot.Portfolio!.AppUserId == userId)
            .Select(snapshot => new PositionSnapshotDto
            {
                Id = snapshot.Id,
                PortfolioId = snapshot.PortfolioId,
                PortfolioName = snapshot.Portfolio!.Name,
                AssetId = snapshot.AssetId,
                AssetName = snapshot.Asset != null ? snapshot.Asset.Name : null,
                AssetSymbol = snapshot.Asset != null ? snapshot.Asset.Symbol : null,
                SnapshotAt = snapshot.SnapshotAt,
                Quantity = snapshot.Quantity,
                AverageCost = snapshot.AverageCost,
                MarketPrice = snapshot.MarketPrice,
                InvestedAmount = snapshot.InvestedAmount,
                MarketValue = snapshot.MarketValue,
                UnrealizedProfit = snapshot.UnrealizedProfit,
                CurrencyCode = snapshot.Asset != null && snapshot.Asset.Currency != null ? snapshot.Asset.Currency.Code : null
            });
    }
}
