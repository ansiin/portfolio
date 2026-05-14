using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1.Watchlists;

public class WatchlistCreateDto
{
    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; } = default!;
}
