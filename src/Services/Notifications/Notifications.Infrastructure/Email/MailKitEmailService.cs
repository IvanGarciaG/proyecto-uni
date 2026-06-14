using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Notifications.Application;
using Microsoft.Extensions.Logging;

namespace Notifications.Infrastructure.Email;

public class MailKitEmailService(SmtpOptions options, ILogger<MailKitEmailService> logger) : IEmailService
{
    public async Task SendAsync(EmailMessage message, CancellationToken ct)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(options.FromName, options.FromAddress));
        mime.To.Add(new MailboxAddress(message.ToName, message.To));
        mime.Subject = message.Subject;
        mime.Body = new BodyBuilder { HtmlBody = message.HtmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(options.Host, options.Port,
            options.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable, ct);
        await client.AuthenticateAsync(options.Username, options.Password, ct);
        await client.SendAsync(mime, ct);
        await client.DisconnectAsync(true, ct);

        logger.LogInformation("Email enviado a {To}: {Subject}", message.To, message.Subject);
    }
}

public class SmtpOptions
{
    public const string Section = "Smtp";
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FromAddress { get; init; } = string.Empty;
    public string FromName { get; init; } = string.Empty;
    public bool UseSsl { get; init; } = false;
}
