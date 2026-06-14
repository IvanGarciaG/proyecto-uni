using MediatR;
using Shared.Kernel;

namespace Geocoding.Application.Queries;

public record ForwardGeocodeQuery(string Address) : IRequest<Result<CoordinatesDto>>;

public record CoordinatesDto(double Latitude, double Longitude, string FormattedAddress);

public class ForwardGeocodeQueryHandler(IGeocodingService geocoding)
    : IRequestHandler<ForwardGeocodeQuery, Result<CoordinatesDto>>
{
    public async Task<Result<CoordinatesDto>> Handle(ForwardGeocodeQuery query, CancellationToken ct)
    {
        var coords = await geocoding.ForwardGeocodeAsync(query.Address, ct);
        if (coords is null)
            return Result<CoordinatesDto>.Failure("No se encontraron coordenadas para la dirección indicada.");
        return Result<CoordinatesDto>.Success(coords);
    }
}
