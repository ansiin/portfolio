using System.ComponentModel.DataAnnotations;

namespace DentalSaaS.Web.ViewModels.Patients;

public sealed class PatientFormViewModel
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(-18));
}
