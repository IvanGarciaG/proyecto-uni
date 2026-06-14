namespace Shared.Contracts.Events;

public record ReportGeneratedEvent(
    Guid ReportId,
    Guid UserId,
    string UserEmail,
    string UserName,
    string ReportTitle,
    string ReportDescription,
    string ReportUrl,
    DateTime GeneratedAt);
