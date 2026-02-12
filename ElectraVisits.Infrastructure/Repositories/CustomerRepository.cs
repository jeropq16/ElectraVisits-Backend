using ElectraVisits.Domain.Entities;
using ElectraVisits.Domain.Interfaces;
using ElectraVisits.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectraVisits.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;
    public CustomerRepository(AppDbContext db) => _db = db;

    public Task<Customer?> GetByNicAsync(string nic, CancellationToken ct = default)
        => _db.Customers.FirstOrDefaultAsync(c => c.Nic == nic, ct);

    public Task AddAsync(Customer customer, CancellationToken ct = default)
        => _db.Customers.AddAsync(customer, ct).AsTask();
}