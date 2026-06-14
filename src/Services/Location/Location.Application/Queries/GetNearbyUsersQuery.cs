using MediatR;
using Shared.Kernel;

namespace Location.Application.Queries;

public record GetNearbyUsersQuery(
    double Latitude,
    double Longitude,
    double RadiusKm) : IRequest<Result<IReadOnlyList<NearbyUserDto>>>;

public record NearbyUserDto(Guid UserId, double Latitude, double Longitude, double DistanceKm, DateTime RecordedAt);

public class GetNearbyUsersQueryHandler(ILocationRepository repository)
    : IRequestHandler<GetNearbyUsersQuery, Result<IReadOnlyList<NearbyUserDto>>>
{
    public async Task<Result<IReadOnlyList<NearbyUserDto>>> Handle(GetNearbyUsersQuery query, CancellationToken ct)
    {
        // Obtiene la última posición de cada usuario y filtra por radio
        var nearby = await repository.GetLatestLocationsNearAsync(
            query.Latitude, query.Longitude, query.RadiusKm, ct);

        return Result<IReadOnlyList<NearbyUserDto>>.Success(nearby);
    }
}
