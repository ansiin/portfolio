using App.DAL.EF;
using App.Domain;
using App.DTO.Admin.Lookups;
using Base.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class AdminLookupService
{
    private readonly AppDbContext _context;

    public AdminLookupService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AdminCurrencyDto>> GetCurrenciesAsync()
    {
        var entities = await _context.Currencies
            .OrderBy(entity => entity.Code)
            .ToListAsync();

        return entities.Select(MapCurrency).ToList();
    }

    public async Task<AdminCurrencyDto?> GetCurrencyAsync(Guid id)
    {
        var entity = await _context.Currencies
            .FirstOrDefaultAsync(item => item.Id == id);

        return entity == null ? null : MapCurrency(entity);
    }

    public async Task<AdminCurrencyDto> CreateCurrencyAsync(AdminCurrencyCreateDto dto)
    {
        var normalizedCode = NormalizeUpperCode(dto.Code, "Code");
        await EnsureCurrencyCodeAvailableAsync(normalizedCode, null);

        var entity = new Currency
        {
            Code = normalizedCode,
            Symbol = NormalizeOptional(dto.Symbol),
            DisplayName = CreateLangStr(dto.DisplayNameEn, dto.DisplayNameEt),
            IsActive = dto.IsActive
        };

        _context.Currencies.Add(entity);
        await _context.SaveChangesAsync();

        return await GetCurrencyAsync(entity.Id)
               ?? throw new InvalidOperationException("Created currency could not be loaded.");
    }

    public async Task<bool> UpdateCurrencyAsync(Guid id, AdminCurrencyUpdateDto dto)
    {
        var entity = await _context.Currencies
            .AsTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (entity == null)
        {
            return false;
        }

        var normalizedCode = NormalizeUpperCode(dto.Code, "Code");
        await EnsureCurrencyCodeAvailableAsync(normalizedCode, id);

        entity.Code = normalizedCode;
        entity.Symbol = NormalizeOptional(dto.Symbol);
        entity.DisplayName = CreateLangStr(dto.DisplayNameEn, dto.DisplayNameEt);
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<AdminLookupDto>> GetAssetTypesAsync()
    {
        var entities = await _context.AssetTypes
            .OrderBy(entity => entity.Code)
            .ToListAsync();

        return entities.Select(MapLookup).ToList();
    }

    public async Task<AdminLookupDto?> GetAssetTypeAsync(Guid id)
    {
        var entity = await _context.AssetTypes
            .FirstOrDefaultAsync(item => item.Id == id);

        return entity == null ? null : MapLookup(entity);
    }

    public async Task<AdminLookupDto> CreateAssetTypeAsync(AdminLookupCreateDto dto)
    {
        var normalizedCode = NormalizeLowerCode(dto.Code, "Code");
        await EnsureAssetTypeCodeAvailableAsync(normalizedCode, null);

        var entity = new AssetType
        {
            Code = normalizedCode,
            DisplayName = CreateLangStr(dto.DisplayNameEn, dto.DisplayNameEt),
            IsActive = dto.IsActive
        };

        _context.AssetTypes.Add(entity);
        await _context.SaveChangesAsync();

        return await GetAssetTypeAsync(entity.Id)
               ?? throw new InvalidOperationException("Created asset type could not be loaded.");
    }

    public async Task<bool> UpdateAssetTypeAsync(Guid id, AdminLookupUpdateDto dto)
    {
        var entity = await _context.AssetTypes
            .AsTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (entity == null)
        {
            return false;
        }

        var normalizedCode = NormalizeLowerCode(dto.Code, "Code");
        await EnsureAssetTypeCodeAvailableAsync(normalizedCode, id);

        entity.Code = normalizedCode;
        entity.DisplayName = CreateLangStr(dto.DisplayNameEn, dto.DisplayNameEt);
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<AdminLookupDto>> GetExchangesAsync()
    {
        var entities = await _context.Exchanges
            .OrderBy(entity => entity.Code)
            .ToListAsync();

        return entities.Select(MapLookup).ToList();
    }

    public async Task<AdminLookupDto?> GetExchangeAsync(Guid id)
    {
        var entity = await _context.Exchanges
            .FirstOrDefaultAsync(item => item.Id == id);

        return entity == null ? null : MapLookup(entity);
    }

    public async Task<AdminLookupDto> CreateExchangeAsync(AdminLookupCreateDto dto)
    {
        var normalizedCode = NormalizeUpperCode(dto.Code, "Code");
        await EnsureExchangeCodeAvailableAsync(normalizedCode, null);

        var entity = new Exchange
        {
            Code = normalizedCode,
            DisplayName = CreateLangStr(dto.DisplayNameEn, dto.DisplayNameEt),
            IsActive = dto.IsActive
        };

        _context.Exchanges.Add(entity);
        await _context.SaveChangesAsync();

        return await GetExchangeAsync(entity.Id)
               ?? throw new InvalidOperationException("Created exchange could not be loaded.");
    }

    public async Task<bool> UpdateExchangeAsync(Guid id, AdminLookupUpdateDto dto)
    {
        var entity = await _context.Exchanges
            .AsTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (entity == null)
        {
            return false;
        }

        var normalizedCode = NormalizeUpperCode(dto.Code, "Code");
        await EnsureExchangeCodeAvailableAsync(normalizedCode, id);

        entity.Code = normalizedCode;
        entity.DisplayName = CreateLangStr(dto.DisplayNameEn, dto.DisplayNameEt);
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<AdminMarketDataProviderDto>> GetMarketDataProvidersAsync()
    {
        var entities = await _context.MarketDataProviders
            .OrderBy(entity => entity.Code)
            .ToListAsync();

        return entities.Select(MapMarketDataProvider).ToList();
    }

    public async Task<AdminMarketDataProviderDto?> GetMarketDataProviderAsync(Guid id)
    {
        var entity = await _context.MarketDataProviders
            .FirstOrDefaultAsync(item => item.Id == id);

        return entity == null ? null : MapMarketDataProvider(entity);
    }

    public async Task<AdminMarketDataProviderDto> CreateMarketDataProviderAsync(AdminMarketDataProviderCreateDto dto)
    {
        var normalizedCode = NormalizeLowerCode(dto.Code, "Code");
        await EnsureMarketDataProviderCodeAvailableAsync(normalizedCode, null);

        var entity = new MarketDataProvider
        {
            Code = normalizedCode,
            DisplayName = CreateLangStr(dto.DisplayNameEn, dto.DisplayNameEt),
            BaseUrl = NormalizeOptional(dto.BaseUrl),
            IsActive = dto.IsActive
        };

        _context.MarketDataProviders.Add(entity);
        await _context.SaveChangesAsync();

        return await GetMarketDataProviderAsync(entity.Id)
               ?? throw new InvalidOperationException("Created market data provider could not be loaded.");
    }

    public async Task<bool> UpdateMarketDataProviderAsync(Guid id, AdminMarketDataProviderUpdateDto dto)
    {
        var entity = await _context.MarketDataProviders
            .AsTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (entity == null)
        {
            return false;
        }

        var normalizedCode = NormalizeLowerCode(dto.Code, "Code");
        await EnsureMarketDataProviderCodeAvailableAsync(normalizedCode, id);

        entity.Code = normalizedCode;
        entity.DisplayName = CreateLangStr(dto.DisplayNameEn, dto.DisplayNameEt);
        entity.BaseUrl = NormalizeOptional(dto.BaseUrl);
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    private static AdminLookupDto MapLookup(AssetType entity)
    {
        return new AdminLookupDto
        {
            Id = entity.Id,
            Code = entity.Code,
            DisplayNameEn = entity.DisplayName.Translate("en") ?? entity.Code,
            DisplayNameEt = entity.DisplayName.Translate("et") ?? entity.Code,
            IsActive = entity.IsActive
        };
    }

    private static AdminLookupDto MapLookup(Exchange entity)
    {
        return new AdminLookupDto
        {
            Id = entity.Id,
            Code = entity.Code,
            DisplayNameEn = entity.DisplayName.Translate("en") ?? entity.Code,
            DisplayNameEt = entity.DisplayName.Translate("et") ?? entity.Code,
            IsActive = entity.IsActive
        };
    }

    private static AdminCurrencyDto MapCurrency(Currency entity)
    {
        return new AdminCurrencyDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Symbol = entity.Symbol,
            DisplayNameEn = entity.DisplayName.Translate("en") ?? entity.Code,
            DisplayNameEt = entity.DisplayName.Translate("et") ?? entity.Code,
            IsActive = entity.IsActive
        };
    }

    private static AdminMarketDataProviderDto MapMarketDataProvider(MarketDataProvider entity)
    {
        return new AdminMarketDataProviderDto
        {
            Id = entity.Id,
            Code = entity.Code,
            DisplayNameEn = entity.DisplayName.Translate("en") ?? entity.Code,
            DisplayNameEt = entity.DisplayName.Translate("et") ?? entity.Code,
            BaseUrl = entity.BaseUrl,
            IsActive = entity.IsActive
        };
    }

    private async Task EnsureCurrencyCodeAvailableAsync(string code, Guid? id)
    {
        var exists = await _context.Currencies.AnyAsync(entity => entity.Id != id && entity.Code == code);
        if (exists)
        {
            throw new InvalidOperationException("Currency code already exists.");
        }
    }

    private async Task EnsureAssetTypeCodeAvailableAsync(string code, Guid? id)
    {
        var exists = await _context.AssetTypes.AnyAsync(entity => entity.Id != id && entity.Code == code);
        if (exists)
        {
            throw new InvalidOperationException("Asset type code already exists.");
        }
    }

    private async Task EnsureExchangeCodeAvailableAsync(string code, Guid? id)
    {
        var exists = await _context.Exchanges.AnyAsync(entity => entity.Id != id && entity.Code == code);
        if (exists)
        {
            throw new InvalidOperationException("Exchange code already exists.");
        }
    }

    private async Task EnsureMarketDataProviderCodeAvailableAsync(string code, Guid? id)
    {
        var exists = await _context.MarketDataProviders.AnyAsync(entity => entity.Id != id && entity.Code == code);
        if (exists)
        {
            throw new InvalidOperationException("Market data provider code already exists.");
        }
    }

    private static LangStr CreateLangStr(string englishValue, string estonianValue)
    {
        if (string.IsNullOrWhiteSpace(englishValue) || string.IsNullOrWhiteSpace(estonianValue))
        {
            throw new InvalidOperationException("English and Estonian display names are required.");
        }

        var result = new LangStr(englishValue.Trim(), "en");
        result.SetTranslation(estonianValue.Trim(), "et");
        return result;
    }

    private static string NormalizeUpperCode(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{fieldName} is required.");
        }

        return value.Trim().ToUpperInvariant();
    }

    private static string NormalizeLowerCode(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{fieldName} is required.");
        }

        return value.Trim().ToLowerInvariant();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
