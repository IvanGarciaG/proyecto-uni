using Shared.Kernel;

namespace Users.Domain.Events;

public record UserCreatedDomainEvent(
    Guid UserId,
    string Email,
    string FullName) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserDeactivatedDomainEvent(Guid UserId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
