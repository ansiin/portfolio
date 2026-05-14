using Base.Domain;

namespace App.Domain;

public class WatchlistItem : BaseEntity
{
    public Guid WatchlistId { get; set; }
    public Watchlist? Watchlist { get; set; }

    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }
}
