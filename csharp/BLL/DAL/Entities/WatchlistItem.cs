namespace DAL.Entities;

public class WatchlistItem
{
    public long WatchlistId { get; set; }
    public long AssetId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Watchlist Watchlist { get; set; } = null!;
    public Asset Asset { get; set; } = null!;
}
