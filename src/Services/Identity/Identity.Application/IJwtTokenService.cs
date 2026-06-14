namespace Identity.Application;

public interface IJwtTokenService
{
    JwtResult GenerateToken(AuthUser user);
    string GenerateRefreshToken();
}

public record JwtResult(string AccessToken, DateTime ExpiresAt);

public class AuthUser
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
}
