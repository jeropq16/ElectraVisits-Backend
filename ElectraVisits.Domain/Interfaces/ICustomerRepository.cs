using ElectraVisits.Domain.Entities;

namespace ElectraVisits.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByNicAsync(string nic, CancellationToken ct = default);
    Task AddAsync(Customer customer, CancellationToken ct = default);
}