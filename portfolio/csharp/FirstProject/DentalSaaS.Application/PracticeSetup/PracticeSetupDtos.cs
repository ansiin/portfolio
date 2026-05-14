namespace DentalSaaS.Application.PracticeSetup;

public sealed record TreatmentRoomDto(Guid Id, string Name);
public sealed record TreatmentTypeDto(Guid Id, string Name, int DurationMinutes, decimal Price);
public sealed record DentistDto(Guid Id, string Name, string LicenseNumber);
