namespace DentalSaaS.Application.Treatments;

public sealed record TreatmentDto(
    Guid Id,
    Guid PatientId,
    Guid TreatmentTypeId,
    DateTimeOffset PerformedAt,
    decimal Cost);

public sealed record CreateTreatmentRequest(Guid PatientId, Guid TreatmentTypeId, DateTimeOffset PerformedAt, decimal Cost);

public sealed record UpdateTreatmentRequest(Guid Id, Guid TreatmentTypeId, DateTimeOffset PerformedAt, decimal Cost);
