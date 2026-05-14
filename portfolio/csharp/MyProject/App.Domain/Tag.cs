using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class Tag : BaseEntity
{
    [StringLength(64, MinimumLength = 1)]
    public string Name { get; set; } = default!;

    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public ICollection<AssetTag>? AssetTags { get; set; }
}
