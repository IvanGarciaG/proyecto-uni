using MediatR;
using Shared.Kernel;

namespace Users.Application.Users.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>;

public record UserDto(Guid Id, string Email, string FullName, bool IsActive, DateTime CreatedAt);

public class GetUserByIdQueryHandler(IUserRepository repository)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(query.UserId, ct);
        if (user is null)
            return Result<UserDto>.Failure("User not found.");

        return new UserDto(user.Id, user.Email, user.FullName, user.IsActive, user.CreatedAt);
    }
}
