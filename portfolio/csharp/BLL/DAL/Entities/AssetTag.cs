namespace DAL.Entities;

public class AssetTag
{
    public long AssetId { get; set; }
    public long TagId { get; set; }

    public Asset Asset { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
