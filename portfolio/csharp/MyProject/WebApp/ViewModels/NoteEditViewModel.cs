using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class NoteEditViewModel
{
    public Guid Id { get; set; }

    [StringLength(128)]
    public string? Title { get; set; }

    [MaxLength(4000)]
    public string Content { get; set; } = default!;

    public Guid? AssetId { get; set; }
    public Guid? TransactionId { get; set; }

    [ValidateNever]
    public List<SelectListItem> AssetSelectList { get; set; } = new();

    [ValidateNever]
    public List<SelectListItem> TransactionSelectList { get; set; } = new();
}
