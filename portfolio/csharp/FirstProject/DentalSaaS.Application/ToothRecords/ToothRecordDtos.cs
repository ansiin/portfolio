namespace DentalSaaS.Application.ToothRecords;

public sealed record ToothRecordDto(
    Guid Id,
    Guid PatientId,
    int ToothNumber,
    string ConditionStatus,
    string? Notes,
    DateTimeOffset CreatedAt);

public sealed record CreateToothRecordRequest(Guid PatientId, int ToothNumber, string ConditionStatus, string? Notes);

public sealed record UpdateToothRecordRequest(Guid Id, int ToothNumber, string ConditionStatus, string? Notes);
