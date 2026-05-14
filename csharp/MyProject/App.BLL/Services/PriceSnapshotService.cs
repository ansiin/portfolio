using App.BLL.Abstractions;
using App.DAL.EF;
using App.Domain;
using App.DTO.v1.PriceSnapshots;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class PriceSnapshotService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public PriceSnapshotService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<PriceSnapshotDto>> GetMyPriceSnapshotsAsync(Guid? assetId = null)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var query = BuildQuery(userId);

        if (assetId.HasValue)
        {
            query = query.Where(snapshot => snapshot.AssetId == assetId.Value);
        }

        return await query
            .OrderByDescending(snapshot => snapshot.RecordedAt)
            .ToListAsync();
    }

    public async Task<PriceSnapshotDto?> GetMyPriceSnapshotAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        return await BuildQuery(userId)
            .FirstOrDefaultAsync(snapshot => snapshot.Id == id);
    }

    public async Task<PriceSnapshotDto> CreateAsync(PriceSnapshotCreateDto dto)
    {
        var userId = _currentUserService.GetRequiredUserId();
        await ValidateAsync(userId, dto.AssetId, dto.CurrencyId, dto.MarketDataProviderId, dto.Price);

        var entity = new PriceSnapshot
        {
            AssetId = dto.AssetId,
            CurrencyId = dto.CurrencyId,
            MarketDataProviderId = dto.MarketDataProviderId,
            RecordedAt = dto.RecordedAt,
            Price = dto.Price
        };

        _context.PriceSnapshots.Add(entity);
        await _context.SaveChangesAsync();

        return await GetRequiredSnapshotAsync(entity.Id, userId);
    }

    public async Task<bool> UpdateAsync(Guid id, PriceSnapshotUpdateDto dto)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.PriceSnapshots
            .Include(snapshot => snapshot.Asset)!
            .ThenInclude(asset => asset!.Portfolio)
            .AsTracking()
            .FirstOrDefaultAsync(snapshot => snapshot.Id == id && snapshot.Asset!.Portfolio!.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        await ValidateAsync(userId, dto.AssetId, dto.CurrencyId, dto.MarketDataProviderId, dto.Price);

        entity.AssetId = dto.AssetId;
        entity.CurrencyId = dto.CurrencyId;
        entity.MarketDataProviderId = dto.MarketDataProviderId;
        entity.RecordedAt = dto.RecordedAt;
        entity.Price = dto.Price;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.PriceSnapshots
            .Include(snapshot => snapshot.Asset)!
            .ThenInclude(asset => asset!.Portfolio)
            .AsTracking()
            .FirstOrDefaultAsync(snapshot => snapshot.Id == id && snapshot.Asset!.Portfolio!.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        _context.PriceSnapshots.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<PriceSnapshotDto> BuildQuery(Guid userId)
    {
        return _context.PriceSnapshots
            .Where(snapshot => snapshot.Asset!.Portfolio!.AppUserId == userId)
            .Select(snapshot => new PriceSnapshotDto
            {
                Id = snapshot.Id,
                AssetId = snapshot.AssetId,
                AssetName = snapshot.Asset!.Name,
                AssetSymbol = snapshot.Asset.Symbol,
                PortfolioName = snapshot.Asset.Portfolio!.Name,
                CurrencyId = snapshot.CurrencyId,
                CurrencyCode = snapshot.Currency!.Code,
                MarketDataProviderId = snapshot.MarketDataProviderId,
                MarketDataProviderCode = snapshot.MarketDataProvider != null ? snapshot.MarketDataProvider.Code : null,
                RecordedAt = snapshot.RecordedAt,
                Price = snapshot.Price
            });
    }

    private async Task ValidateAsync(Guid userId, Guid assetId, Guid currencyId, Guid? providerId, decimal price)
    {
        if (price <= 0)
        {
            throw new InvalidOperationException("Price must be greater than zero.");
        }

        var assetExists = await _context.Assets.AnyAsync(asset =>
            asset.Id == assetId &&
            asset.Portfolio!.AppUserId == userId);

        if (!assetExists)
        {
            throw new KeyNotFoundException("Asset not found.");
        }

        var currencyExists = await _context.Currencies.AnyAsync(currency =>
            currency.Id == currencyId &&
            currency.IsActive);

        if (!currencyExists)
        {
            throw new KeyNotFoundException("Currency not found.");
        }

        if (providerId.HasValue)
        {
            var providerExists = await _context.MarketDataProviders.AnyAsync(provider =>
                provider.Id == providerId.Value &&
                provider.IsActive);

            if (!providerExists)
            {
                throw new KeyNotFoundException("Market data provider not found.");
            }
        }
    }

    private async Task<PriceSnapshotDto> GetRequiredSnapshotAsync(Guid id, Guid userId)
    {
        return await BuildQuery(userId)
                   .FirstOrDefaultAsync(snapshot => snapshot.Id == id)
               ?? throw new InvalidOperationException("Created price snapshot could not be loaded.");
    }
}
