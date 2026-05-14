namespace App.DTO.v1.Assets;

public class AssetDto
{
    public Guid Id { get; set; }
    public Guid PortfolioId { get; set; }
    public string PortfolioName { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Symbol { get; set; }
    public Guid AssetTypeId { get; set; }
    public string AssetTypeCode { get; set; } = default!;
    public Guid CurrencyId { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public Guid? ExchangeId { get; set; }
    public string? ExchangeCode { get; set; }
    public Guid? MarketDataProviderId { get; set; }
    public string? MarketDataProviderCode { get; set; }
    public bool IsActive { get; set; }
}
