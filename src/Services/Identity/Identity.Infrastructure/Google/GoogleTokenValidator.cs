using Google.Apis.Auth;
using Identity.Application;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Google;

public class GoogleTokenValidator(GoogleAuthOptions options, ILogger<GoogleTokenValidator> logger)
    : IGoogleTokenValidator
{
    public async Task<GoogleUserInfo?> ValidateAsync(string idToken, CancellationToken ct)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [options.ClientId]
                });

            return new GoogleUserInfo(
                payload.Subject,
                payload.Email,
                payload.Name,
                payload.Picture,
                payload.EmailVerified);
        }
        catch (InvalidJwtException ex)
        {
            logger.LogWarning(ex, "Token de Google inválido o expirado");
            return null;
        }
    }
}

public class GoogleAuthOptions
{
    public const string Section = "GoogleAuth";
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
}
