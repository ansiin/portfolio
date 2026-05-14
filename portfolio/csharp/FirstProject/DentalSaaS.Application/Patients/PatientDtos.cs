namespace DentalSaaS.Application.Patients;

public sealed record PatientListItem(Guid Id, string FullName, DateOnly DateOfBirth, string Email);

public sealed record CreatePatientRequest(string FirstName, string LastName, DateOnly DateOfBirth, string Email);

public sealed record UpdatePatientRequest(Guid Id, string FirstName, string LastName, DateOnly DateOfBirth, string Email);
