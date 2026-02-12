using ElectraVisits.Application.DTOs.Auth;

namespace ElectraVisits.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    Task<AuthResponseDto> RefreshAsync(RefreshRequestDto dto);
    Task LogoutAsync(string refreshToken);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto); 
}