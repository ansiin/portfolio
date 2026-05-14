using DentalSaaS.Shared.Models;

namespace DentalSaaS.Application.Appointments;

public interface IAppointmentService
{
    Task<IReadOnlyCollection<AppointmentItemDto>> ListAsync(CancellationToken ct = default);
    Task<Result<Guid>> CreateAsync(CreateAppointmentRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
