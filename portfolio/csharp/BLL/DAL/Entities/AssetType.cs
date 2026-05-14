namespace DAL.Entities;

public class AssetType
{
    public long Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;

    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
