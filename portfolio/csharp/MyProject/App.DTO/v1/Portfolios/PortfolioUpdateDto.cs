using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1.Portfolios;

public class PortfolioUpdateDto
{
    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; } = default!;

    public Guid BaseCurrencyId { get; set; }
    public bool IsArchived { get; set; }
}
