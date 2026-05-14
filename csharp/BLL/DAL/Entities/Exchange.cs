namespace DAL.Entities;

public class Exchange
{
    public long Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Country { get; set; }
    public string? Timezone { get; set; }

    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
