using ElectraVisits.Domain.Entities;
using ElectraVisits.Domain.Interfaces;
using ElectraVisits.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectraVisits.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;
    public RefreshTokenRepository(AppDbContext db) => _db = db;

    public Task<RefreshToken?> GetByTokenWithUserAsync(string token, CancellationToken ct = default)
        => _db.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.Token == token, ct);

    public Task AddAsync(RefreshToken token, CancellationToken ct = default)
        => _db.RefreshTokens.AddAsync(token, ct).AsTask();

    public Task RevokeAsync(RefreshToken token, CancellationToken ct = default)
    {
        token.RevokedAtUtc = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}