namespace Identity.Application;

public interface IGoogleTokenValidator
{
    Task<GoogleUserInfo?> ValidateAsync(string idToken, CancellationToken ct);
}

public record GoogleUserInfo(string GoogleId, string Email, string FullName, string? AvatarUrl, bool EmailVerified);
