using ElectraVisits.Application.DTOs.Customers;

namespace ElectraVisits.Application.Interfaces;

public interface ICustomerService
{
    Task<object> CreateAsync(CreateCustomerDto dto, CancellationToken ct = default);
    Task<object?> GetByNicAsync(string nic, CancellationToken ct = default);
}