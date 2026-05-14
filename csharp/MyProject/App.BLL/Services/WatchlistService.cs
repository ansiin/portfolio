using App.BLL.Abstractions;
using App.DAL.EF;
using App.Domain;
using App.DTO.v1.Watchlists;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class WatchlistService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public WatchlistService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<WatchlistDto>> GetMyWatchlistsAsync()
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entities = await BuildQuery(userId)
            .OrderBy(watchlist => watchlist.Name)
            .ToListAsync();

        return entities.Select(MapWatchlist).ToList();
    }

    public async Task<WatchlistDto?> GetMyWatchlistAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await BuildQuery(userId)
            .FirstOrDefaultAsync(watchlist => watchlist.Id == id);

        return entity == null ? null : MapWatchlist(entity);
    }

    public async Task<WatchlistDto> CreateAsync(WatchlistCreateDto dto)
    {
        var entity = new Watchlist
        {
            AppUserId = _currentUserService.GetRequiredUserId(),
            Name = dto.Name.Trim()
        };

        _context.Watchlists.Add(entity);
        await _context.SaveChangesAsync();

        return await GetRequiredWatchlistAsync(entity.Id);
    }

    public async Task<bool> UpdateAsync(Guid id, WatchlistUpdateDto dto)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.Watchlists
            .AsTracking()
            .FirstOrDefaultAsync(watchlist => watchlist.Id == id && watchlist.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        entity.Name = dto.Name.Trim();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.Watchlists
            .Include(watchlist => watchlist.Items)
            .AsTracking()
            .FirstOrDefaultAsync(watchlist => watchlist.Id == id && watchlist.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        if (entity.Items != null && entity.Items.Count > 0)
        {
            _context.WatchlistItems.RemoveRange(entity.Items);
        }

        _context.Watchlists.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<WatchlistItemDto> AddItemAsync(Guid watchlistId, WatchlistItemCreateDto dto)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var watchlist = await _context.Watchlists
            .Include(entity => entity.Items)
            .AsTracking()
            .FirstOrDefaultAsync(entity => entity.Id == watchlistId && entity.AppUserId == userId);

        if (watchlist == null)
        {
            throw new KeyNotFoundException("Watchlist not found.");
        }

        var asset = await _context.Assets
            .Include(entity => entity.Portfolio)
            .FirstOrDefaultAsync(entity => entity.Id == dto.AssetId && entity.Portfolio!.AppUserId == userId);

        if (asset == null)
        {
            throw new KeyNotFoundException("Asset not found.");
        }

        var exists = watchlist.Items?.Any(item => item.AssetId == dto.AssetId) ?? false;
        if (exists)
        {
            throw new InvalidOperationException("Asset is already in this watchlist.");
        }

        var item = new WatchlistItem
        {
            WatchlistId = watchlistId,
            AssetId = dto.AssetId
        };

        _context.WatchlistItems.Add(item);
        await _context.SaveChangesAsync();

        return new WatchlistItemDto
        {
            Id = item.Id,
            AssetId = asset.Id,
            AssetName = asset.Name,
            AssetSymbol = asset.Symbol,
            PortfolioName = asset.Portfolio!.Name,
            AssetIsActive = asset.IsActive
        };
    }

    public async Task<bool> RemoveItemAsync(Guid watchlistId, Guid itemId)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.WatchlistItems
            .Include(item => item.Watchlist)
            .AsTracking()
            .FirstOrDefaultAsync(item =>
                item.Id == itemId &&
                item.WatchlistId == watchlistId &&
                item.Watchlist!.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        _context.WatchlistItems.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<Watchlist> BuildQuery(Guid userId)
    {
        return _context.Watchlists
            .Where(watchlist => watchlist.AppUserId == userId)
            .Include(watchlist => watchlist.Items)!
            .ThenInclude(item => item.Asset)!
            .ThenInclude(asset => asset!.Portfolio);
    }

    private async Task<WatchlistDto> GetRequiredWatchlistAsync(Guid id)
    {
        return await GetMyWatchlistAsync(id)
               ?? throw new InvalidOperationException("Created watchlist could not be loaded.");
    }

    private static WatchlistDto MapWatchlist(Watchlist entity)
    {
        var items = entity.Items?
            .Where(item => item.Asset != null && item.Asset.Portfolio != null)
            .OrderBy(item => item.Asset!.Name)
            .Select(item => new WatchlistItemDto
            {
                Id = item.Id,
                AssetId = item.AssetId,
                AssetName = item.Asset!.Name,
                AssetSymbol = item.Asset.Symbol,
                PortfolioName = item.Asset.Portfolio!.Name,
                AssetIsActive = item.Asset.IsActive
            })
            .ToList() ?? new List<WatchlistItemDto>();

        return new WatchlistDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ItemCount = items.Count,
            Items = items
        };
    }
}
