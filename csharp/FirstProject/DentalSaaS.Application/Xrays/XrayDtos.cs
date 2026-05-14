namespace DentalSaaS.Application.Xrays;

public sealed record XrayDto(
    Guid Id,
    Guid PatientId,
    DateTimeOffset TakenAt,
    DateTimeOffset DueAt,
    bool IsOverdue,
    int OverdueDays,
    string FileUrl);

public sealed record CreateXrayRequest(Guid PatientId, DateTimeOffset TakenAt, string FileUrl);

public sealed record UpdateXrayRequest(Guid Id, DateTimeOffset TakenAt, string FileUrl);

public sealed record XrayOverdueSummaryDto(int OverduePatientCount, int TrackedPatientCount, int XrayIntervalDays);
