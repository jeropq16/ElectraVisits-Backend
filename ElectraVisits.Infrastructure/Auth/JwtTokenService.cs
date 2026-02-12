using ElectraVisits.Application.Interfaces;
using ElectraVisits.Domain.Entities;
using Microsoft.Extensions.Options;

namespace ElectraVisits.Infrastructure.Auth;

public class JwtTokenService : ITokenService
{
    private readonly TokenService _inner;
    private readonly JwtSettings _jwt;

    public JwtTokenService(IOptions<JwtSettings> jwt, TokenService inner)
    {
        _jwt = jwt.Value;
        _inner = inner;
    }

    public int RefreshTokenDays => _jwt.RefreshTokenDays;

    public (string token, int expiresInSeconds) CreateAccessToken(User user, string[] roles)
        => _inner.CreateAccessToken(user, roles);

    public string CreateRefreshToken()
        => _inner.CreateRefreshToken();
}