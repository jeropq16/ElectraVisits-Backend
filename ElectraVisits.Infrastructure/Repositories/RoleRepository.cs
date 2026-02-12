using ElectraVisits.Domain.Entities;
using ElectraVisits.Domain.Interfaces;
using ElectraVisits.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectraVisits.Infrastructure.Repositories;

public class RoleRepository :  IRoleRepository
{
    private readonly AppDbContext _db;
    public RoleRepository(AppDbContext db) => _db = db;

    public Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
        => _db.Roles.FirstOrDefaultAsync(r => r.Name == name, ct);

    public Task AddAsync(Role role, CancellationToken ct = default)
        => _db.Roles.AddAsync(role, ct).AsTask();
}