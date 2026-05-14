namespace App.DTO.v1.Watchlists;

public class WatchlistItemDto
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = default!;
    public string? AssetSymbol { get; set; }
    public string PortfolioName { get; set; } = default!;
    public bool AssetIsActive { get; set; }
}
