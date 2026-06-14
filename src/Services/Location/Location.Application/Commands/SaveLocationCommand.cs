using Location.Domain.Entities;
using MediatR;
using Shared.Kernel;

namespace Location.Application.Commands;

public record SaveLocationCommand(
    Guid UserId,
    double Latitude,
    double Longitude,
    double? AccuracyMeters,
    string? Label) : IRequest<Result<Guid>>;

public class SaveLocationCommandHandler(ILocationRepository repository)
    : IRequestHandler<SaveLocationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SaveLocationCommand cmd, CancellationToken ct)
    {
        var location = UserLocation.Create(cmd.UserId, cmd.Latitude, cmd.Longitude, cmd.AccuracyMeters, cmd.Label);
        await repository.AddAsync(location, ct);
        await repository.SaveChangesAsync(ct);
        return Result<Guid>.Success(location.Id);
    }
}
