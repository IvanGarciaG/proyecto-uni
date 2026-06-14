using MediatR;
using Shared.Kernel;
using Users.Domain.Entities;

namespace Users.Application.Users.Commands;

public record RegisterUserCommand(string Email, string FullName, string Password) : IRequest<Result<Guid>>;

public class RegisterUserCommandHandler(IUserRepository repository, IPasswordHasher hasher, IPublishEndpoint bus)
    : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand cmd, CancellationToken ct)
    {
        if (await repository.ExistsByEmailAsync(cmd.Email, ct))
            return Result<Guid>.Failure("Email already registered.");

        var hash = hasher.Hash(cmd.Password);
        var user = User.Create(cmd.Email, cmd.FullName, hash);

        await repository.AddAsync(user, ct);
        await repository.SaveChangesAsync(ct);

        // Publica evento de integración al message bus
        await bus.Publish(new Shared.Contracts.Events.UserRegisteredEvent(
            user.Id, user.Email, user.FullName, user.CreatedAt), ct);

        return Result<Guid>.Success(user.Id);
    }
}
