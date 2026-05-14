using App.BLL.Abstractions;
using App.DAL.EF;
using App.Domain;
using App.Domain.Enums;
using App.DTO.v1.Transactions;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class TransactionService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public TransactionService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<TransactionDto>> GetMyTransactionsAsync(Guid? portfolioId = null)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var query = _context.Transactions
            .Include(transaction => transaction.Portfolio)
            .Include(transaction => transaction.Asset)
            .Include(transaction => transaction.Fees)
            .Where(transaction => transaction.Portfolio!.AppUserId == userId);

        if (portfolioId.HasValue)
        {
            query = query.Where(transaction => transaction.PortfolioId == portfolioId.Value);
        }

        var entities = await query
            .OrderByDescending(transaction => transaction.ExecutedAt)
            .ToListAsync();

        return entities.Select(MapTransaction).ToList();
    }

    public async Task<TransactionDto?> GetMyTransactionAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.Transactions
            .Include(transaction => transaction.Portfolio)
            .Include(transaction => transaction.Asset)
            .Include(transaction => transaction.Fees)
            .FirstOrDefaultAsync(transaction => transaction.Id == id && transaction.Portfolio!.AppUserId == userId);

        return entity == null ? null : MapTransaction(entity);
    }

    public async Task<TransactionDto> CreateAsync(TransactionCreateDto dto)
    {
        var userId = _currentUserService.GetRequiredUserId();
        await ValidateTransactionAsync(userId, dto.PortfolioId, dto.AssetId, dto.Type);

        var entity = new Transaction
        {
            PortfolioId = dto.PortfolioId,
            AssetId = NormalizeAssetId(dto.AssetId, dto.Type),
            Type = dto.Type,
            ExecutedAt = dto.ExecutedAt,
            Quantity = NormalizeQuantity(dto.Type, dto.Quantity),
            UnitPrice = NormalizeUnitPrice(dto.Type, dto.UnitPrice),
            TotalAmount = dto.TotalAmount,
            Description = NormalizeOptional(dto.Description),
            Fees = dto.Fees.Select(MapFeeInput).ToList()
        };

        _context.Transactions.Add(entity);
        await _context.SaveChangesAsync();

        return await GetRequiredTransactionAsync(entity.Id, userId);
    }

    public async Task<bool> UpdateAsync(Guid id, TransactionUpdateDto dto)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.Transactions
            .Include(transaction => transaction.Portfolio)
            .Include(transaction => transaction.Fees)
            .AsTracking()
            .FirstOrDefaultAsync(transaction => transaction.Id == id && transaction.Portfolio!.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        await ValidateTransactionAsync(userId, dto.PortfolioId, dto.AssetId, dto.Type);

        entity.PortfolioId = dto.PortfolioId;
        entity.AssetId = NormalizeAssetId(dto.AssetId, dto.Type);
        entity.Type = dto.Type;
        entity.ExecutedAt = dto.ExecutedAt;
        entity.Quantity = NormalizeQuantity(dto.Type, dto.Quantity);
        entity.UnitPrice = NormalizeUnitPrice(dto.Type, dto.UnitPrice);
        entity.TotalAmount = dto.TotalAmount;
        entity.Description = NormalizeOptional(dto.Description);

        if (entity.Fees != null && entity.Fees.Count > 0)
        {
            _context.TransactionFees.RemoveRange(entity.Fees);
        }

        entity.Fees = dto.Fees.Select(MapFeeInput).ToList();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.Transactions
            .Include(transaction => transaction.Portfolio)
            .Include(transaction => transaction.Fees)
            .AsTracking()
            .FirstOrDefaultAsync(transaction => transaction.Id == id && transaction.Portfolio!.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        if (entity.Fees != null && entity.Fees.Count > 0)
        {
            _context.TransactionFees.RemoveRange(entity.Fees);
        }

        _context.Transactions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task ValidateTransactionAsync(Guid userId, Guid portfolioId, Guid? assetId, TransactionType type)
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

        if (RequiresAsset(type) && !assetId.HasValue)
        {
            throw new InvalidOperationException("Selected transaction type requires an asset.");
        }

        if (!AllowsAsset(type) && assetId.HasValue)
        {
            throw new InvalidOperationException("Selected transaction type does not allow an asset.");
        }

        if (assetId.HasValue)
        {
            var assetExists = await _context.Assets.AnyAsync(asset =>
                asset.Id == assetId.Value &&
                asset.PortfolioId == portfolioId &&
                asset.Portfolio!.AppUserId == userId);

            if (!assetExists)
            {
                throw new KeyNotFoundException("Asset not found in selected portfolio.");
            }
        }
    }

    private async Task<TransactionDto> GetRequiredTransactionAsync(Guid id, Guid userId)
    {
        var entity = await _context.Transactions
            .Include(transaction => transaction.Portfolio)
            .Include(transaction => transaction.Asset)
            .Include(transaction => transaction.Fees)
            .FirstOrDefaultAsync(transaction => transaction.Id == id && transaction.Portfolio!.AppUserId == userId);

        return entity == null
            ? throw new InvalidOperationException("Created transaction could not be loaded.")
            : MapTransaction(entity);
    }

    private static TransactionDto MapTransaction(Transaction entity)
    {
        var fees = entity.Fees?
            .Select(fee => new TransactionFeeDto
            {
                Id = fee.Id,
                FeeType = fee.FeeType,
                Amount = fee.Amount
            })
            .ToList() ?? new List<TransactionFeeDto>();

        return new TransactionDto
        {
            Id = entity.Id,
            PortfolioId = entity.PortfolioId,
            PortfolioName = entity.Portfolio?.Name ?? string.Empty,
            AssetId = entity.AssetId,
            AssetName = entity.Asset?.Name,
            Type = entity.Type,
            ExecutedAt = entity.ExecutedAt,
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice,
            TotalAmount = entity.TotalAmount,
            Description = entity.Description,
            FeeTotal = fees.Sum(fee => fee.Amount),
            Fees = fees
        };
    }

    private static TransactionFee MapFeeInput(TransactionFeeInputDto dto)
    {
        return new TransactionFee
        {
            FeeType = dto.FeeType.Trim(),
            Amount = dto.Amount
        };
    }

    private static Guid? NormalizeAssetId(Guid? assetId, TransactionType type)
    {
        return AllowsAsset(type) ? assetId : null;
    }

    private static decimal NormalizeQuantity(TransactionType type, decimal quantity)
    {
        return RequiresAsset(type) ? quantity : 0m;
    }

    private static decimal NormalizeUnitPrice(TransactionType type, decimal unitPrice)
    {
        return RequiresAsset(type) ? unitPrice : 0m;
    }

    private static bool RequiresAsset(TransactionType type)
    {
        return type is TransactionType.Buy or TransactionType.Sell;
    }

    private static bool AllowsAsset(TransactionType type)
    {
        return type is TransactionType.Buy or TransactionType.Sell or TransactionType.Dividend;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
