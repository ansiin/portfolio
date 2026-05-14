namespace DAL.Entities;

public class CorporateAction
{
    public long Id { get; set; }
    public long AssetId { get; set; }
    public string ActionType { get; set; } = null!;
    public DateOnly ActionDate { get; set; }
    public decimal? RatioFrom { get; set; }
    public decimal? RatioTo { get; set; }
    public string? Note { get; set; }

    public Asset Asset { get; set; } = null!;
}
