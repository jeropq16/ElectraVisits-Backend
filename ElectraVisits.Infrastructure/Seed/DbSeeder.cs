using ElectraVisits.Application.Interfaces;
using ElectraVisits.Domain.Entities;
using ElectraVisits.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ElectraVisits.Infrastructure.Seed;

public class DbSeeder
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly SeedSettings _seed;

    public DbSeeder(AppDbContext db, IPasswordHasher hasher, IOptions<SeedSettings> seed)
    {
        _db = db;
        _hasher = hasher;
        _seed = seed.Value;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var rolesToEnsure = new[] { "Admin", "Operations" };

        foreach (var roleName in rolesToEnsure)
        {
            var exists = await _db.Roles.AnyAsync(r => r.Name == roleName, ct);
            if (!exists)
                _db.Roles.Add(new Role { Name = roleName });
        }

        await _db.SaveChangesAsync(ct);

        var adminEmail = _seed.AdminEmail.Trim().ToLowerInvariant();
        var admin = await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == adminEmail, ct);

        if (admin is null)
        {
            admin = new User
            {
                Email = adminEmail,
                PasswordHash = _hasher.Hash(_seed.AdminPassword),
                IsActive = true
            };
            _db.Users.Add(admin);
            await _db.SaveChangesAsync(ct);
        }

        var adminRole = await _db.Roles.FirstAsync(r => r.Name == "Admin", ct);

        var hasAdminRole = await _db.UserRoles.AnyAsync(ur => ur.UserId == admin.Id && ur.RoleId == adminRole.Id, ct);
        if (!hasAdminRole)
        {
            _db.UserRoles.Add(new UserRole
            {
                UserId = admin.Id,
                RoleId = adminRole.Id
            });
            await _db.SaveChangesAsync(ct);
        }
    }
}
