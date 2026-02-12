using ElectraVisits.Application.DTOs.Appointments;

namespace ElectraVisits.Application.Interfaces;

public interface IAppointmentService
{
    Task<object> CreateAsync(CreateAppointmentDto dto, CancellationToken ct = default);
    Task<List<object>> GetByNicAsync(string nic, CancellationToken ct = default);
    Task<object> UpdateStatusAsync(Guid id, UpdateAppointmentStatusDto dto, CancellationToken ct = default);
}