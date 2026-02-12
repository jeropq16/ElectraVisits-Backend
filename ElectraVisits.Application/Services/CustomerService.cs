using ElectraVisits.Application.DTOs.Customers;
using ElectraVisits.Application.Interfaces;
using ElectraVisits.Domain.Entities;
using ElectraVisits.Domain.Interfaces;

namespace ElectraVisits.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _uow;

    public CustomerService(ICustomerRepository customers, IUnitOfWork uow)
    {
        _customers = customers;
        _uow = uow;
    }

    public async Task<object> CreateAsync(CreateCustomerDto dto, CancellationToken ct = default)
    {
        var nic = dto.Nic.Trim();
        if (string.IsNullOrWhiteSpace(nic)) throw new ArgumentException("NIC requerido.");
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Nombre requerido.");

        var existing = await _customers.GetByNicAsync(nic, ct);
        if (existing is not null) return new { existing.Id, existing.Nic, existing.Name };

        var customer = new Customer { Nic = nic, Name = dto.Name.Trim() };
        await _customers.AddAsync(customer, ct);
        await _uow.SaveChangesAsync(ct);

        return new { customer.Id, customer.Nic, customer.Name };
    }

    public async Task<object?> GetByNicAsync(string nic, CancellationToken ct = default)
    {
        var c = await _customers.GetByNicAsync(nic.Trim(), ct);
        return c is null ? null : new { c.Id, c.Nic, c.Name };
    }
}