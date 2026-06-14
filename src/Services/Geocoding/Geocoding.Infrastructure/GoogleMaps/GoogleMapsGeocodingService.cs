using System.Net.Http.Json;
using Geocoding.Application;
using Geocoding.Application.Queries;
using Microsoft.Extensions.Logging;

namespace Geocoding.Infrastructure.GoogleMaps;

public class GoogleMapsGeocodingService(
    HttpClient http,
    GoogleMapsOptions options,
    ILogger<GoogleMapsGeocodingService> logger) : IGeocodingService
{
    private const string BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";

    public async Task<AddressDto?> ReverseGeocodeAsync(double lat, double lng, CancellationToken ct)
    {
        var url = $"{BaseUrl}?latlng={lat},{lng}&key={options.ApiKey}&language=es";
        return await FetchAddress(url, lat, lng, ct);
    }

    public async Task<CoordinatesDto?> ForwardGeocodeAsync(string address, CancellationToken ct)
    {
        var encoded = Uri.EscapeDataString(address);
        var url = $"{BaseUrl}?address={encoded}&key={options.ApiKey}&language=es";

        try
        {
            var response = await http.GetFromJsonAsync<GoogleGeocodeResponse>(url, ct);
            var result = response?.Results?.FirstOrDefault();
            if (result is null) return null;

            return new CoordinatesDto(
                result.Geometry.Location.Lat,
                result.Geometry.Location.Lng,
                result.FormattedAddress);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en forward geocoding para dirección: {Address}", address);
            return null;
        }
    }

    private async Task<AddressDto?> FetchAddress(string url, double lat, double lng, CancellationToken ct)
    {
        try
        {
            var response = await http.GetFromJsonAsync<GoogleGeocodeResponse>(url, ct);
            var result = response?.Results?.FirstOrDefault();
            if (result is null) return null;

            return new AddressDto(
                result.FormattedAddress,
                GetComponent(result, "route"),
                GetComponent(result, "locality"),
                GetComponent(result, "administrative_area_level_1"),
                GetComponent(result, "country"),
                GetComponent(result, "postal_code"),
                lat, lng);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en reverse geocoding ({Lat},{Lng})", lat, lng);
            return null;
        }
    }

    private static string GetComponent(GoogleResult result, string type) =>
        result.AddressComponents
              .FirstOrDefault(c => c.Types.Contains(type))?.LongName ?? string.Empty;
}

// ── DTOs de respuesta de Google Maps API ──────────────────────────────
public record GoogleGeocodeResponse(List<GoogleResult>? Results, string Status);
public record GoogleResult(
    string FormattedAddress,
    List<AddressComponent> AddressComponents,
    Geometry Geometry);
public record AddressComponent(string LongName, string ShortName, List<string> Types);
public record Geometry(LatLng Location);
public record LatLng(double Lat, double Lng);
