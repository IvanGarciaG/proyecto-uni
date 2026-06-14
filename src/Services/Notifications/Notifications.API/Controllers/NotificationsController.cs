using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notifications.Application.Commands;

namespace Notifications.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    /// <summary>Envía manualmente un aviso de reporte por email. Útil para pruebas o reenvíos.</summary>
    [HttpPost("report-alert")]
    public async Task<IActionResult> SendReportAlert([FromBody] SendReportAlertEmailCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Ok(new { message = "Email enviado." }) : StatusCode(500, new { error = result.Error });
    }
}
