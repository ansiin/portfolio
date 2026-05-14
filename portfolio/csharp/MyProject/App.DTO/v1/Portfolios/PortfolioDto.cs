namespace App.DTO.v1.Portfolios;

public class PortfolioDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid BaseCurrencyId { get; set; }
    public string BaseCurrencyCode { get; set; } = default!;
    public bool IsArchived { get; set; }
}
