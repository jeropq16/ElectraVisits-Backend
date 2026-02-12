namespace ElectraVisits.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nic { get; set; } = default!;
    public string Name { get; set; } = default!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}