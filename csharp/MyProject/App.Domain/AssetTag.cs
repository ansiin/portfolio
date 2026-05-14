using Base.Domain;

namespace App.Domain;

public class AssetTag : BaseEntity
{
    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }

    public Guid TagId { get; set; }
    public Tag? Tag { get; set; }
}
