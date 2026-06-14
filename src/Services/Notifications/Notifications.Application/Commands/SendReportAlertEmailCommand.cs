using MediatR;
using Shared.Kernel;

namespace Notifications.Application.Commands;

public record SendReportAlertEmailCommand(
    string ToEmail,
    string ToName,
    string ReportTitle,
    string ReportDescription,
    string ReportUrl,
    DateTime GeneratedAt) : IRequest<Result>;

public class SendReportAlertEmailCommandHandler(IEmailService emailService, IEmailTemplateRenderer renderer)
    : IRequestHandler<SendReportAlertEmailCommand, Result>
{
    public async Task<Result> Handle(SendReportAlertEmailCommand cmd, CancellationToken ct)
    {
        var body = renderer.Render("ReportAlert", new
        {
            cmd.ToName,
            cmd.ReportTitle,
            cmd.ReportDescription,
            cmd.ReportUrl,
            GeneratedAt = cmd.GeneratedAt.ToString("dd/MM/yyyy HH:mm"),
        });

        await emailService.SendAsync(new EmailMessage(
            To: cmd.ToEmail,
            ToName: cmd.ToName,
            Subject: $"Nuevo reporte disponible: {cmd.ReportTitle}",
            HtmlBody: body), ct);

        return Result.Success();
    }
}
