namespace App.DTO.v1.Notes;

public class NoteDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public Guid? AssetId { get; set; }
    public string? AssetName { get; set; }
    public Guid? TransactionId { get; set; }
    public string? TransactionLabel { get; set; }
}
