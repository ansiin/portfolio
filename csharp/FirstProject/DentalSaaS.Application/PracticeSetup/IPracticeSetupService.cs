using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.PracticeSetup;

public interface IPracticeSetupService
{
    Task<IReadOnlyCollection<TreatmentRoomDto>> ListRoomsAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<TreatmentTypeDto>> ListTypesAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<DentistDto>> ListDentistsAsync(CancellationToken ct = default);
    Task<Result<Guid>> CreateRoomAsync(string name, CancellationToken ct = default);
    Task<Result<Guid>> CreateTypeAsync(string name, int durationMinutes, decimal price, CancellationToken ct = default);
    Task<Result<Guid>> CreateDentistAsync(string name, string licenseNumber, CancellationToken ct = default);
    Task<Result> DeleteRoomAsync(Guid id, CancellationToken ct = default);
    Task<Result> DeleteTypeAsync(Guid id, CancellationToken ct = default);
    Task<Result> DeleteDentistAsync(Guid id, CancellationToken ct = default);
}
