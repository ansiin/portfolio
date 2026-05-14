using DentalSaaS.Domain.Common;
using DentalSaaS.Shared.Enums;

namespace DentalSaaS.Domain.Entities;

public sealed class PlanItem : CompanyEntityBase
{
    public Guid TreatmentPlanId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
    public int Sequence { get; set; }
    public int Urgency { get; set; }
    public PlanItemDecisionStatus DecisionStatus { get; set; } = PlanItemDecisionStatus.Pending;
}
