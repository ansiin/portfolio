using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class PositionSnapshotGenerateViewModel
{
    public Guid? PortfolioId { get; set; }
    public DateTime SnapshotAt { get; set; } = DateTime.UtcNow;

    [ValidateNever]
    public List<SelectListItem> PortfolioSelectList { get; set; } = new();
}
