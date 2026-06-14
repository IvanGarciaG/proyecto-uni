using MediatR;
using Shared.Kernel;

namespace Identity.Application.Commands;

public record GoogleLoginCommand(string IdToken) : IRequest<Result<AuthTokenDto>>;

public record AuthTokenDto(string AccessToken, string RefreshToken, DateTime ExpiresAt, UserInfoDto User);

public record UserInfoDto(Guid UserId, string Email, string FullName, string? AvatarUrl);

public class GoogleLoginCommandHandler(
    IGoogleTokenValidator googleValidator,
    IUserAuthRepository userRepo,
    IJwtTokenService jwtService)
    : IRequestHandler<GoogleLoginCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(GoogleLoginCommand cmd, CancellationToken ct)
    {
        // 1. Validar token de Google
        var googleUser = await googleValidator.ValidateAsync(cmd.IdToken, ct);
        if (googleUser is null)
            return Result<AuthTokenDto>.Failure("Token de Google inválido.");

        // 2. Buscar o crear usuario
        var user = await userRepo.FindByEmailAsync(googleUser.Email, ct)
                   ?? await userRepo.CreateFromGoogleAsync(googleUser, ct);

        // 3. Emitir JWT propio
        var token = jwtService.GenerateToken(user);
        var refresh = jwtService.GenerateRefreshToken();

        await userRepo.SaveRefreshTokenAsync(user.Id, refresh, ct);

        return Result<AuthTokenDto>.Success(new AuthTokenDto(
            token.AccessToken,
            refresh,
            token.ExpiresAt,
            new UserInfoDto(user.Id, user.Email, user.FullName, user.AvatarUrl)));
    }
}
