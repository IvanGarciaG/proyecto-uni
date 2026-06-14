using MassTransit;
using MediatR;
using Notifications.Application.Commands;
using Shared.Contracts.Events;

namespace Notifications.Infrastructure.Messaging;

/// <summary>
/// Escucha el evento ReportGeneratedEvent del bus y dispara el email automáticamente.
/// </summary>
public class ReportGeneratedConsumer(IMediator mediator) : IConsumer<ReportGeneratedEvent>
{
    public async Task Consume(ConsumeContext<ReportGeneratedEvent> context)
    {
        var evt = context.Message;

        await mediator.Send(new SendReportAlertEmailCommand(
            evt.UserEmail,
            evt.UserName,
            evt.ReportTitle,
            evt.ReportDescription,
            evt.ReportUrl,
            evt.GeneratedAt));
    }
}
