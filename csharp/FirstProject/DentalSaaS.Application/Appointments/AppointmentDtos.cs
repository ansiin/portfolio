namespace DentalSaaS.Application.Appointments;

public sealed record AppointmentItemDto(
    Guid Id,
    Guid PatientId,
    Guid TreatmentRoomId,
    Guid DentistId,
    Guid? PlanItemId,
    string? PlanItemDescription,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    string TypeName);

public sealed record CreateAppointmentRequest(
    Guid PatientId,
    Guid TreatmentRoomId,
    Guid DentistId,
    Guid? PlanItemId,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    string TypeName);
