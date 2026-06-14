using MediatR;
using Shared.Kernel;

namespace Geocoding.Application.Queries;

public record ReverseGeocodeQuery(double Latitude, double Longitude) : IRequest<Result<AddressDto>>;

public record AddressDto(
    string FormattedAddress,
    string Street,
    string City,
    string State,
    string Country,
    string PostalCode,
    double Latitude,
    double Longitude);

public class ReverseGeocodeQueryHandler(IGeocodingService geocoding)
    : IRequestHandler<ReverseGeocodeQuery, Result<AddressDto>>
{
    public async Task<Result<AddressDto>> Handle(ReverseGeocodeQuery query, CancellationToken ct)
    {
        var address = await geocoding.ReverseGeocodeAsync(query.Latitude, query.Longitude, ct);
        if (address is null)
            return Result<AddressDto>.Failure("No se encontró dirección para las coordenadas indicadas.");
        return Result<AddressDto>.Success(address);
    }
}
