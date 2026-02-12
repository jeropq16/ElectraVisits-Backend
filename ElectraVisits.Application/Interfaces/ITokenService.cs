using ElectraVisits.Domain.Entities;

namespace ElectraVisits.Application.Interfaces;

public interface ITokenService
{
    (string token, int expiresInSeconds) CreateAccessToken(User user, string[] roles);
    string CreateRefreshToken();
    int RefreshTokenDays { get; }
}