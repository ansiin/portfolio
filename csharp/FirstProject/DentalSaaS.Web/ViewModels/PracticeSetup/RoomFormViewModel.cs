using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.PracticeSetup;

public sealed class RoomFormViewModel
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;
}
