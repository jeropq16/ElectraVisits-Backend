using ElectraVisits.Domain.Entities;
using ElectraVisits.Domain.Interfaces;
using ElectraVisits.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectraVisits.Infrastructure.Repositories;

public class UserRepository :IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db)  => _db = db;

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.AnyAsync(u => u.Email == email, ct);

    public Task AddAsync(User user, CancellationToken ct = default)
        => _db.Users.AddAsync(user, ct).AsTask();

    public Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken ct = default)
        => _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
}