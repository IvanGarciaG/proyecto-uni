using Geocoding.Application.Queries;

namespace Geocoding.Application;

public interface IGeocodingService
{
    Task<AddressDto?> ReverseGeocodeAsync(double lat, double lng, CancellationToken ct);
    Task<CoordinatesDto?> ForwardGeocodeAsync(string address, CancellationToken ct);
}
