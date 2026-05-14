using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class Watchlist : BaseEntity
{
    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; } = default!;

    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public ICollection<WatchlistItem>? Items { get; set; }
}
