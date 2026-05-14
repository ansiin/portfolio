namespace App.DTO.v1.PositionSnapshots;

public class PositionSnapshotGenerateDto
{
    public Guid? PortfolioId { get; set; }
    public DateTime SnapshotAt { get; set; } = DateTime.UtcNow;
}
