namespace App.DTO.v1.Lookups;

public class CurrencyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string? Symbol { get; set; }
    public string DisplayName { get; set; } = default!;
}
