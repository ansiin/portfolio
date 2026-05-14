using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class AssetType : BaseEntity
{
    [StringLength(32, MinimumLength = 1)]
    public string Code { get; set; } = default!;

    public LangStr DisplayName { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    public ICollection<Asset>? Assets { get; set; }
}
