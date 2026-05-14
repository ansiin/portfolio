namespace App.DTO.v1.Lookups;

public class LookupItemDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
}
