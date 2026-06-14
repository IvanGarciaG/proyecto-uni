using Location.Domain.Entities;
using Location.Application.Queries;

namespace Location.Application;

public interface ILocationRepository
{
    Task AddAsync(UserLocation location, CancellationToken ct);
    Task<UserLocation?> GetLatestByUserIdAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<NearbyUserDto>> GetLatestLocationsNearAsync(double lat, double lng, double radiusKm, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
