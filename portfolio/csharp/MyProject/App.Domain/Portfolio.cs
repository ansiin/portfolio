using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class Portfolio : BaseEntity
{
    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; } = default!;

    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public Guid BaseCurrencyId { get; set; }
    public Currency? BaseCurrency { get; set; }

    public bool IsArchived { get; set; }

    public ICollection<Asset>? Assets { get; set; }
    public ICollection<Transaction>? Transactions { get; set; }
    public ICollection<PositionSnapshot>? PositionSnapshots { get; set; }
}
