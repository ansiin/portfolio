using App.BLL.Abstractions;
using App.DAL.EF;
using App.Domain;
using App.DTO.v1.Notes;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class NoteService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public NoteService(AppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<NoteDto>> GetMyNotesAsync(Guid? assetId = null, Guid? transactionId = null)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var query = BuildQuery(userId);

        if (assetId.HasValue)
        {
            query = query.Where(note => note.AssetId == assetId.Value);
        }

        if (transactionId.HasValue)
        {
            query = query.Where(note => note.TransactionId == transactionId.Value);
        }

        var entities = await query
            .OrderByDescending(note => note.CreatedAt)
            .ToListAsync();

        return entities.Select(MapNote).ToList();
    }

    public async Task<NoteDto?> GetMyNoteAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await BuildQuery(userId)
            .FirstOrDefaultAsync(note => note.Id == id);

        return entity == null ? null : MapNote(entity);
    }

    public async Task<NoteDto> CreateAsync(NoteCreateDto dto)
    {
        var userId = _currentUserService.GetRequiredUserId();
        await ValidateReferencesAsync(userId, dto.AssetId, dto.TransactionId, dto.Content);

        var entity = new Note
        {
            AppUserId = userId,
            AssetId = dto.AssetId,
            TransactionId = dto.TransactionId,
            Title = NormalizeOptional(dto.Title),
            Content = dto.Content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Notes.Add(entity);
        await _context.SaveChangesAsync();

        return await GetRequiredNoteAsync(entity.Id);
    }

    public async Task<bool> UpdateAsync(Guid id, NoteUpdateDto dto)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.Notes
            .AsTracking()
            .FirstOrDefaultAsync(note => note.Id == id && note.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        await ValidateReferencesAsync(userId, dto.AssetId, dto.TransactionId, dto.Content);

        entity.AssetId = dto.AssetId;
        entity.TransactionId = dto.TransactionId;
        entity.Title = NormalizeOptional(dto.Title);
        entity.Content = dto.Content.Trim();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var entity = await _context.Notes
            .AsTracking()
            .FirstOrDefaultAsync(note => note.Id == id && note.AppUserId == userId);

        if (entity == null)
        {
            return false;
        }

        _context.Notes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<Note> BuildQuery(Guid userId)
    {
        return _context.Notes
            .Where(note => note.AppUserId == userId)
            .Include(note => note.Asset)
            .Include(note => note.Transaction)!
            .ThenInclude(transaction => transaction!.Asset);
    }

    private async Task ValidateReferencesAsync(Guid userId, Guid? assetId, Guid? transactionId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("Content is required.");
        }

        if (assetId.HasValue)
        {
            var assetExists = await _context.Assets.AnyAsync(asset =>
                asset.Id == assetId.Value &&
                asset.Portfolio!.AppUserId == userId);

            if (!assetExists)
            {
                throw new KeyNotFoundException("Asset not found.");
            }
        }

        if (transactionId.HasValue)
        {
            var transactionExists = await _context.Transactions.AnyAsync(transaction =>
                transaction.Id == transactionId.Value &&
                transaction.Portfolio!.AppUserId == userId);

            if (!transactionExists)
            {
                throw new KeyNotFoundException("Transaction not found.");
            }
        }
    }

    private async Task<NoteDto> GetRequiredNoteAsync(Guid id)
    {
        return await GetMyNoteAsync(id)
               ?? throw new InvalidOperationException("Created note could not be loaded.");
    }

    private static NoteDto MapNote(Note entity)
    {
        string? transactionLabel = null;
        if (entity.Transaction != null)
        {
            transactionLabel = entity.Transaction.Asset?.Name is { Length: > 0 }
                ? $"{entity.Transaction.Type} {entity.Transaction.ExecutedAt:yyyy-MM-dd} ({entity.Transaction.Asset.Name})"
                : $"{entity.Transaction.Type} {entity.Transaction.ExecutedAt:yyyy-MM-dd}";
        }

        return new NoteDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content,
            CreatedAt = entity.CreatedAt,
            AssetId = entity.AssetId,
            AssetName = entity.Asset?.Name,
            TransactionId = entity.TransactionId,
            TransactionLabel = transactionLabel
        };
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
