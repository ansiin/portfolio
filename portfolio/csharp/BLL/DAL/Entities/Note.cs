namespace DAL.Entities;

public class Note
{
    public long Id { get; set; }
    public string AppUserId { get; set; } = null!;
    public long? AssetId { get; set; }
    public long? TransactionId { get; set; }
    public string Content { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }

    public Asset? Asset { get; set; }
    public Transaction? Transaction { get; set; }
}
