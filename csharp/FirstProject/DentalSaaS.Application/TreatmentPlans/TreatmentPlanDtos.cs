using DentalSaaS.Shared.Enums;

namespace DentalSaaS.Application.TreatmentPlans;

public sealed record PlanItemDto(
    Guid Id,
    string Description,
    decimal EstimatedCost,
    int Sequence,
    int Urgency,
    PlanItemDecisionStatus Status,
    int ScheduledAppointments,
    DateTimeOffset? NextScheduledAt);
public sealed record TreatmentPlanDto(Guid Id, Guid PatientId, string Title, IReadOnlyCollection<PlanItemDto> Items);
public sealed record CreateTreatmentPlanRequest(Guid PatientId, string Title);
public sealed record AddPlanItemRequest(Guid PlanId, string Description, decimal EstimatedCost, int Sequence, int Urgency);
