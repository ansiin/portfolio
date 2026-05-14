using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class Note : BaseEntity
{
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public Guid? AssetId { get; set; }
    public Asset? Asset { get; set; }

    public Guid? TransactionId { get; set; }
    public Transaction? Transaction { get; set; }

    [StringLength(128)]
    public string? Title { get; set; }

    [MaxLength(4000)]
    public string Content { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
