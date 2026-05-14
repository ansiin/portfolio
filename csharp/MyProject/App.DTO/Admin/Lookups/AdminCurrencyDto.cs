namespace App.DTO.Admin.Lookups;

public class AdminCurrencyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string? Symbol { get; set; }
    public string DisplayNameEn { get; set; } = default!;
    public string DisplayNameEt { get; set; } = default!;
    public bool IsActive { get; set; }
}
