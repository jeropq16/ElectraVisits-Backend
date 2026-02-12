using ElectraVisits.Domain.Entities;
using ElectraVisits.Domain.Interfaces;
using ElectraVisits.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectraVisits.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _db;
    public AppointmentRepository(AppDbContext db) => _db = db;

    public Task<bool> ExistsAsync(Guid customerId, DateOnly date, int timeSlot, CancellationToken ct = default)
        => _db.Appointments.AnyAsync(a =>
            a.CustomerId == customerId &&
            a.Date == date &&
            (int)a.TimeSlot == timeSlot, ct);

    public Task AddAsync(Appointment appointment, CancellationToken ct = default)
        => _db.Appointments.AddAsync(appointment, ct).AsTask();

    public Task<List<Appointment>> GetByNicAsync(string nic, CancellationToken ct = default)
        => _db.Appointments
            .Include(a => a.Customer)
            .Where(a => a.Customer.Nic == nic)
            .OrderByDescending(a => a.Date)
            .ToListAsync(ct);

    public Task<Appointment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Appointments.Include(a => a.Customer).FirstOrDefaultAsync(a => a.Id == id, ct);
}