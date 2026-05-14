namespace DAL.Entities;

public class Tag
{
    public long Id { get; set; }
    public string AppUserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Color { get; set; }

    public ICollection<AssetTag> AssetTags { get; set; } = new List<AssetTag>();
}
