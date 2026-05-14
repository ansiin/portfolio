using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1.Assets;

public class AssetCreateDto
{
    public Guid PortfolioId { get; set; }

    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; } = default!;

    [StringLength(64)]
    public string? Symbol { get; set; }

    public Guid AssetTypeId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? ExchangeId { get; set; }
    public Guid? MarketDataProviderId { get; set; }
}
