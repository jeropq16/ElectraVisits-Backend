using ElectraVisits.Application.DTOs.Appointments;
using ElectraVisits.Application.Interfaces;
using ElectraVisits.Domain.Entities;
using ElectraVisits.Domain.Enums;
using ElectraVisits.Domain.Interfaces;

namespace ElectraVisits.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly ICustomerRepository _customers;
    private readonly IAppointmentRepository _appointments;
    private readonly IUnitOfWork _uow;

    public AppointmentService(ICustomerRepository customers, IAppointmentRepository appointments, IUnitOfWork uow)
    {
        _customers = customers;
        _appointments = appointments;
        _uow = uow;
    }

    public async Task<object> CreateAsync(CreateAppointmentDto dto, CancellationToken ct = default)
    {
        var nic = dto.Nic.Trim();
        if (string.IsNullOrWhiteSpace(nic)) throw new ArgumentException("NIC requerido.");
        if (dto.Date < DateOnly.FromDateTime(DateTime.UtcNow.AddHours(-5))) 
            throw new ArgumentException("La fecha no puede ser pasada.");
        if (dto.TimeSlot is not (1 or 2)) throw new ArgumentException("TimeSlot inválido");

        var customer = await _customers.GetByNicAsync(nic, ct);
        if (customer is null)
        {
            customer = new Customer { Nic = nic, Name = (dto.CustomerName ?? "NA").Trim() };
            await _customers.AddAsync(customer, ct);
        }

        var exists = await _appointments.ExistsAsync(customer.Id, dto.Date, dto.TimeSlot, ct);
        if (exists) throw new InvalidOperationException("Ya existe una cita para esa fecha y franja horaria.");

        var appt = new Appointment
        {
            Customer = customer,
            Date = dto.Date,
            TimeSlot = (TimeSlot)dto.TimeSlot,
            Status = AppointmentStatus.Confirmed
        };

        await _appointments.AddAsync(appt, ct);
        await _uow.SaveChangesAsync(ct);

        return new
        {
            appt.Id,
            Customer = new { customer.Id, customer.Nic, customer.Name },
            appt.Date,
            TimeSlot = appt.TimeSlot.ToString(),
            Status = appt.Status.ToString()
        };
    }

    public async Task<List<object>> GetByNicAsync(string nic, CancellationToken ct = default)
    {
        var list = await _appointments.GetByNicAsync(nic.Trim(), ct);
        return list.Select(a => (object)new
        {
            a.Id,
            Customer = new { a.Customer.Id, a.Customer.Nic, a.Customer.Name },
            a.Date,
            TimeSlot = a.TimeSlot.ToString(),
            Status = a.Status.ToString()
        }).ToList();
    }

    public async Task<object> UpdateStatusAsync(Guid id, UpdateAppointmentStatusDto dto, CancellationToken ct = default)
    {
        if (dto.Status is not (1 or 2)) throw new ArgumentException("Status inválido");

        var appt = await _appointments.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Cita no encontrada.");

        appt.Status = (AppointmentStatus)dto.Status;
        await _uow.SaveChangesAsync(ct);

        return new { appt.Id, appt.Status };
    }
}
