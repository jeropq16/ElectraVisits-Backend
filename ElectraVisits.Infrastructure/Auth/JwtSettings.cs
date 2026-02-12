namespace ElectraVisits.Infrastructure.Auth;

public class JwtSettings
{
    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int ExpMinutes { get; set; } = 60;

    public int RefreshTokenDays { get; set; } = 7;
}