using Shared.Kernel;

namespace Location.Domain.Entities;

public class UserLocation : Entity
{
    public Guid UserId { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public double? AccuracyMeters { get; private set; }
    public string? Label { get; private set; }   // "Casa", "Trabajo", etc.
    public DateTime RecordedAt { get; private set; }

    private UserLocation() { }

    public static UserLocation Create(Guid userId, double lat, double lng, double? accuracy = null, string? label = null)
    {
        if (lat < -90 || lat > 90) throw new ArgumentOutOfRangeException(nameof(lat));
        if (lng < -180 || lng > 180) throw new ArgumentOutOfRangeException(nameof(lng));

        return new UserLocation
        {
            UserId = userId,
            Latitude = lat,
            Longitude = lng,
            AccuracyMeters = accuracy,
            Label = label,
            RecordedAt = DateTime.UtcNow
        };
    }
}
