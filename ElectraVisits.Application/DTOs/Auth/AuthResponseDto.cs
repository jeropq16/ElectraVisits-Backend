namespace ElectraVisits.Application.DTOs.Auth;

public record AuthResponseDto(string AccessToken,
    string RefreshToken,
    int ExpiresInSeconds,
    string[] Roles
    );