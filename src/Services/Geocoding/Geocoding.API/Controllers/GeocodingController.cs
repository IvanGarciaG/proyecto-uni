using Geocoding.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geocoding.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GeocodingController(IMediator mediator) : ControllerBase
{
    /// <summary>Convierte coordenadas GPS a dirección legible.</summary>
    [HttpGet("reverse")]
    public async Task<IActionResult> Reverse(
        [FromQuery] double lat,
        [FromQuery] double lng,
        CancellationToken ct)
    {
        var result = await mediator.Send(new ReverseGeocodeQuery(lat, lng), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>Convierte una dirección a coordenadas GPS.</summary>
    [HttpGet("forward")]
    public async Task<IActionResult> Forward([FromQuery] string address, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(address))
            return BadRequest(new { error = "La dirección no puede estar vacía." });

        var result = await mediator.Send(new ForwardGeocodeQuery(address), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }
}
