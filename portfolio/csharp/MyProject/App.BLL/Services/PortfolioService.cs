using App.BLL.Abstractions;
using App.DAL.EF;
using App.Domain;
using App.DTO.v1.Portfolios;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class PortfolioService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public PortfolioService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<PortfolioDto>> GetMyPortfoliosAsync()
    {
        var userId = _currentUserService.GetRequiredUserId();

        return await BuildPortfolioQuery(userId)
            .OrderBy(portfolio => portfolio.Name)
            .ToListAsync();
    }

    public async Task<PortfolioDto?> GetMyPortfolioAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();

        return await BuildPortfolioQuery(userId)
            .FirstOrDefaultAsync(portfolio => portfolio.Id == id);
    }

    public async Task<PortfolioDto> CreateAsync(PortfolioCreateDto dto)
    {
        await EnsureCurrencyExistsAsync(dto.BaseCurrencyId);

        var entity = new Portfolio
        {
            AppUserId = _currentUserService.GetRequiredUserId(),
            BaseCurrencyId = dto.BaseCurrencyId,
            Name = dto.Name.Trim()
        };

        _context.Portfolios.Add(entity);
        await _context.SaveChangesAsync();

        return await GetCreatedOrUpdatedPortfolioAsync(entity.Id);
    }

    public async Task<bool> UpdateAsync(Guid id, PortfolioUpdateDto dto)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.Portfolios
            .AsTracking()
            .FirstOrDefaultAsync(portfolio => portfolio.Id == id && portfolio.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        await EnsureCurrencyExistsAsync(dto.BaseCurrencyId);

        entity.Name = dto.Name.Trim();
        entity.BaseCurrencyId = dto.BaseCurrencyId;
        entity.IsArchived = dto.IsArchived;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.Portfolios
            .AsTracking()
            .FirstOrDefaultAsync(portfolio => portfolio.Id == id && portfolio.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        var hasRelatedData = await _context.Assets.AnyAsync(asset => asset.PortfolioId == id) ||
                             await _context.Transactions.AnyAsync(transaction => transaction.PortfolioId == id);
        if (hasRelatedData)
        {
            throw new InvalidOperationException("Portfolio with related assets or transactions cannot be deleted.");
        }

        _context.Portfolios.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<PortfolioDto> BuildPortfolioQuery(Guid userId)
    {
        return _context.Portfolios
            .Where(portfolio => portfolio.AppUserId == userId)
            .Select(portfolio => new PortfolioDto
            {
                Id = portfolio.Id,
                Name = portfolio.Name,
                BaseCurrencyId = portfolio.BaseCurrencyId,
                BaseCurrencyCode = portfolio.BaseCurrency!.Code,
                IsArchived = portfolio.IsArchived
            });
    }

    private async Task EnsureCurrencyExistsAsync(Guid currencyId)
    {
        var exists = await _context.Currencies.AnyAsync(currency => currency.Id == currencyId && currency.IsActive);
        if (!exists)
        {
            throw new KeyNotFoundException("Base currency not found.");
        }
    }

    private async Task<PortfolioDto> GetCreatedOrUpdatedPortfolioAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        return await BuildPortfolioQuery(userId)
                   .FirstOrDefaultAsync(portfolio => portfolio.Id == id)
               ?? throw new InvalidOperationException("Created portfolio could not be loaded.");
    }
}
