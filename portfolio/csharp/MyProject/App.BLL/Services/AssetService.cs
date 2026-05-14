using App.BLL.Abstractions;
using App.DAL.EF;
using App.Domain;
using App.DTO.v1.Assets;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class AssetService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AssetService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<AssetDto>> GetMyAssetsAsync(Guid? portfolioId = null)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var query = BuildAssetQuery(userId);

        if (portfolioId.HasValue)
        {
            query = query.Where(asset => asset.PortfolioId == portfolioId.Value);
        }

        return await query
            .OrderBy(asset => asset.Name)
            .ToListAsync();
    }

    public async Task<AssetDto?> GetMyAssetAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        return await BuildAssetQuery(userId)
            .FirstOrDefaultAsync(asset => asset.Id == id);
    }

    public async Task<AssetDto> CreateAsync(AssetCreateDto dto)
    {
        var userId = _currentUserService.GetRequiredUserId();

        await EnsurePortfolioOwnedAsync(dto.PortfolioId, userId);
        await EnsureAssetLookupsExistAsync(dto.AssetTypeId, dto.CurrencyId, dto.ExchangeId, dto.MarketDataProviderId);

        var entity = new Asset
        {
            PortfolioId = dto.PortfolioId,
            Name = dto.Name.Trim(),
            Symbol = NormalizeOptional(dto.Symbol),
            AssetTypeId = dto.AssetTypeId,
            CurrencyId = dto.CurrencyId,
            ExchangeId = dto.ExchangeId,
            MarketDataProviderId = dto.MarketDataProviderId,
            IsActive = true
        };

        _context.Assets.Add(entity);
        await _context.SaveChangesAsync();

        return await GetRequiredAssetAsync(entity.Id, userId);
    }

    public async Task<bool> UpdateAsync(Guid id, AssetUpdateDto dto)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.Assets
            .Include(asset => asset.Portfolio)
            .AsTracking()
            .FirstOrDefaultAsync(asset => asset.Id == id && asset.Portfolio!.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        await EnsureAssetLookupsExistAsync(dto.AssetTypeId, dto.CurrencyId, dto.ExchangeId, dto.MarketDataProviderId);

        entity.Name = dto.Name.Trim();
        entity.Symbol = NormalizeOptional(dto.Symbol);
        entity.AssetTypeId = dto.AssetTypeId;
        entity.CurrencyId = dto.CurrencyId;
        entity.ExchangeId = dto.ExchangeId;
        entity.MarketDataProviderId = dto.MarketDataProviderId;
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.Assets
            .Include(asset => asset.Portfolio)
            .AsTracking()
            .FirstOrDefaultAsync(asset => asset.Id == id && asset.Portfolio!.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        entity.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<AssetDto> BuildAssetQuery(Guid userId)
    {
        return _context.Assets
            .Where(asset => asset.Portfolio!.AppUserId == userId)
            .Select(asset => new AssetDto
            {
                Id = asset.Id,
                PortfolioId = asset.PortfolioId,
                PortfolioName = asset.Portfolio!.Name,
                Name = asset.Name,
                Symbol = asset.Symbol,
                AssetTypeId = asset.AssetTypeId,
                AssetTypeCode = asset.AssetType!.Code,
                CurrencyId = asset.CurrencyId,
                CurrencyCode = asset.Currency!.Code,
                ExchangeId = asset.ExchangeId,
                ExchangeCode = asset.Exchange != null ? asset.Exchange.Code : null,
                MarketDataProviderId = asset.MarketDataProviderId,
                MarketDataProviderCode = asset.MarketDataProvider != null ? asset.MarketDataProvider.Code : null,
                IsActive = asset.IsActive
            });
    }

    private async Task EnsurePortfolioOwnedAsync(Guid portfolioId, Guid userId)
    {
        var portfolio = await _context.Portfolios
            .FirstOrDefaultAsync(entity => entity.Id == portfolioId && entity.AppUserId == userId);

        if (portfolio == null)
        {
            throw new KeyNotFoundException("Portfolio not found.");
        }

        if (portfolio.IsArchived)
        {
            throw new InvalidOperationException("Archived portfolio cannot be modified.");
        }
    }

    private async Task EnsureAssetLookupsExistAsync(Guid assetTypeId, Guid currencyId, Guid? exchangeId, Guid? providerId)
    {
        if (!await _context.AssetTypes.AnyAsync(entity => entity.Id == assetTypeId && entity.IsActive))
        {
            throw new KeyNotFoundException("Asset type not found.");
        }

        if (!await _context.Currencies.AnyAsync(entity => entity.Id == currencyId && entity.IsActive))
        {
            throw new KeyNotFoundException("Currency not found.");
        }

        if (exchangeId.HasValue &&
            !await _context.Exchanges.AnyAsync(entity => entity.Id == exchangeId.Value && entity.IsActive))
        {
            throw new KeyNotFoundException("Exchange not found.");
        }

        if (providerId.HasValue &&
            !await _context.MarketDataProviders.AnyAsync(entity => entity.Id == providerId.Value && entity.IsActive))
        {
            throw new KeyNotFoundException("Market data provider not found.");
        }
    }

    private async Task<AssetDto> GetRequiredAssetAsync(Guid id, Guid userId)
    {
        return await BuildAssetQuery(userId)
                   .FirstOrDefaultAsync(asset => asset.Id == id)
               ?? throw new InvalidOperationException("Created asset could not be loaded.");
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
