using ElectraVisits.Application.DTOs.Auth;
using ElectraVisits.Application.Interfaces;
using ElectraVisits.Domain.Entities;
using ElectraVisits.Domain.Interfaces;

namespace ElectraVisits.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public AuthService(
        IUserRepository users,
        IRoleRepository roles,
        IRefreshTokenRepository refreshTokens,
        IUnitOfWork uow,
        IPasswordHasher hasher,
        ITokenService tokens)
    {
        _users = users;
        _roles = roles;
        _refreshTokens = refreshTokens;
        _uow = uow;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _users.ExistsByEmailAsync(email))
            throw new InvalidOperationException("El usuario ya existe.");

        var user = new User
        {
            Email = email,
            PasswordHash = _hasher.Hash(dto.Password),
            IsActive = true
        };

        const string defaultRole = "Operations";
        var role = await _roles.GetByNameAsync(defaultRole);
        if (role is null)
        {
            role = new Role { Name = defaultRole };
            await _roles.AddAsync(role);
        }

        user.UserRoles.Add(new UserRole { User = user, Role = role });

        await _users.AddAsync(user);

        var roles = new[] { defaultRole };
        var (access, exp) = _tokens.CreateAccessToken(user, roles);

        var refresh = new RefreshToken
        {
            User = user,
            Token = _tokens.CreateRefreshToken(),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_tokens.RefreshTokenDays)
        };

        await _refreshTokens.AddAsync(refresh);
        await _uow.SaveChangesAsync();

        return new AuthResponseDto(access, refresh.Token, exp, roles);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        if (!_hasher.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var full = await _users.GetByIdWithRolesAsync(user.Id)
                   ?? throw new UnauthorizedAccessException("Credenciales inválidas.");

        var roles = full.UserRoles.Select(x => x.Role.Name).ToArray();
        var (access, exp) = _tokens.CreateAccessToken(full, roles);

        var refresh = new RefreshToken
        {
            UserId = full.Id,
            Token = _tokens.CreateRefreshToken(),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_tokens.RefreshTokenDays)
        };

        await _refreshTokens.AddAsync(refresh);
        await _uow.SaveChangesAsync();

        return new AuthResponseDto(access, refresh.Token, exp, roles);
    }

    public async Task<AuthResponseDto> RefreshAsync(RefreshRequestDto dto)
    {
        var existing = await _refreshTokens.GetByTokenWithUserAsync(dto.RefreshToken);

        if (existing is null || !existing.IsActive || !existing.User.IsActive)
            throw new UnauthorizedAccessException("Refresh token inválido.");

        await _refreshTokens.RevokeAsync(existing);

        var user = existing.User;
        var roles = user.UserRoles.Select(x => x.Role.Name).ToArray();
        var (access, exp) = _tokens.CreateAccessToken(user, roles);

        var newRefresh = new RefreshToken
        {
            UserId = user.Id,
            Token = _tokens.CreateRefreshToken(),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_tokens.RefreshTokenDays)
        };

        await _refreshTokens.AddAsync(newRefresh);
        await _uow.SaveChangesAsync();

        return new AuthResponseDto(access, newRefresh.Token, exp, roles);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var existing = await _refreshTokens.GetByTokenWithUserAsync(refreshToken);
        if (existing is null) return;

        await _refreshTokens.RevokeAsync(existing);
        await _uow.SaveChangesAsync();
    }
}
