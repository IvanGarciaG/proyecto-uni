namespace Notifications.Application;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct);
}

public record EmailMessage(string To, string ToName, string Subject, string HtmlBody);
