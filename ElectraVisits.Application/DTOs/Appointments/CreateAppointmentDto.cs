namespace ElectraVisits.Application.DTOs.Appointments;

public record CreateAppointmentDto(string Nic, string? CustomerName, DateOnly Date, int TimeSlot); 
