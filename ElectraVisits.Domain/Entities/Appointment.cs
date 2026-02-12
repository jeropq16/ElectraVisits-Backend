using ElectraVisits.Domain.Enums;

namespace ElectraVisits.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
    
    public DateOnly Date { get; set; }
    public TimeSlot TimeSlot { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed;
}