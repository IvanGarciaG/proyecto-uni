using Location.Application;
using Location.Application.Queries;
using Location.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Location.Infrastructure.Persistence;

public class LocationRepository(LocationDbContext db) : ILocationRepository
{
    public async Task AddAsync(UserLocation location, CancellationToken ct) =>
        await db.UserLocations.AddAsync(location, ct);

    public Task<UserLocation?> GetLatestByUserIdAsync(Guid userId, CancellationToken ct) =>
        db.UserLocations
          .Where(l => l.UserId == userId)
          .OrderByDescending(l => l.RecordedAt)
          .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<NearbyUserDto>> GetLatestLocationsNearAsync(
        double lat, double lng, double radiusKm, CancellationToken ct)
    {
        // Haversine aproximado usando SQL — obtiene últimas posiciones por usuario
        // y filtra por bounding box primero para eficiencia
        var deltaLat = radiusKm / 111.0;
        var deltaLng = radiusKm / (111.0 * Math.Cos(lat * Math.PI / 180));

        var candidates = await db.UserLocations
            .Where(l =>
                l.Latitude  >= lat - deltaLat && l.Latitude  <= lat + deltaLat &&
                l.Longitude >= lng - deltaLng && l.Longitude <= lng + deltaLng)
            .GroupBy(l => l.UserId)
            .Select(g => g.OrderByDescending(l => l.RecordedAt).First())
            .ToListAsync(ct);

        return candidates
            .Select(l => new NearbyUserDto(
                l.UserId, l.Latitude, l.Longitude,
                Haversine(lat, lng, l.Latitude, l.Longitude),
                l.RecordedAt))
            .Where(d => d.DistanceKm <= radiusKm)
            .OrderBy(d => d.DistanceKm)
            .ToList();
    }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
