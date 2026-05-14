namespace DentalSaaS.Application.Reports;

public sealed record ReportsDashboardDto(
    DateOnly DateFrom,
    DateOnly DateTo,
    int ActivePatients,
    int AppointmentsScheduled,
    decimal RoomUtilizationRate,
    int TreatmentCompletionCount,
    int PlanItemsAcceptedCount,
    int PlanItemsDeferredCount,
    decimal UrgentAcceptanceRate,
    int InsuranceSubmissionCount,
    decimal InsuranceApprovalRate,
    decimal OutstandingReceivables,
    int OverdueXrayPatients,
    decimal PaymentPlanExposure);
