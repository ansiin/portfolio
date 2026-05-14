namespace App.DTO.v1.Watchlists;

public class WatchlistDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int ItemCount { get; set; }
    public IReadOnlyList<WatchlistItemDto> Items { get; set; } = Array.Empty<WatchlistItemDto>();
}
