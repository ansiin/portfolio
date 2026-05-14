namespace DAL.Entities;

public class Watchlist
{
    public long Id { get; set; }
    public string AppUserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<WatchlistItem> WatchlistItems { get; set; } = new List<WatchlistItem>();
}
