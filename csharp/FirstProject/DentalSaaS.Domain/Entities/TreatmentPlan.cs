using DentalSaaS.Domain.Common;

namespace DentalSaaS.Domain.Entities;

public sealed class TreatmentPlan : CompanyEntityBase
{
    public Guid PatientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<PlanItem> Items { get; set; } = [];
}
