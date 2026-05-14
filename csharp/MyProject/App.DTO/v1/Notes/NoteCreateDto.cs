using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1.Notes;

public class NoteCreateDto
{
    [StringLength(128)]
    public string? Title { get; set; }

    [MaxLength(4000)]
    public string Content { get; set; } = default!;

    public Guid? AssetId { get; set; }
    public Guid? TransactionId { get; set; }
}
