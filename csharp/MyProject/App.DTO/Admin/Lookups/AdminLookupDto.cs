namespace App.DTO.Admin.Lookups;

public class AdminLookupDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string DisplayNameEn { get; set; } = default!;
    public string DisplayNameEt { get; set; } = default!;
    public bool IsActive { get; set; }
}
