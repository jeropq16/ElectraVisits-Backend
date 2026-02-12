using ElectraVisits.Domain.Entities;

namespace ElectraVisits.Domain.Interfaces;

public interface IAppointmentRepository
{
    Task<bool> ExistsAsync(Guid customerId, DateOnly date, int timeSlot, CancellationToken ct = default);
    Task AddAsync(Appointment appointment, CancellationToken ct = default);
    Task<List<Appointment>> GetByNicAsync(string nic, CancellationToken ct = default);
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken ct = default);
}