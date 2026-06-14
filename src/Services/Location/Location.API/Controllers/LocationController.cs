using Location.Application.Commands;
using Location.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Location.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationController(IMediator mediator) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Guarda la posición actual del usuario autenticado.</summary>
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SaveLocationRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new SaveLocationCommand(CurrentUserId, req.Latitude, req.Longitude, req.AccuracyMeters, req.Label), ct);

        return result.IsSuccess ? Ok(new { locationId = result.Value }) : BadRequest(result.Error);
    }

    /// <summary>Devuelve usuarios cercanos a unas coordenadas en un radio dado.</summary>
    [HttpGet("nearby")]
    public async Task<IActionResult> Nearby(
        [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] double radiusKm = 5,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetNearbyUsersQuery(lat, lng, radiusKm), ct);
        return Ok(result.Value);
    }
}

public record SaveLocationRequest(double Latitude, double Longitude, double? AccuracyMeters, string? Label);
