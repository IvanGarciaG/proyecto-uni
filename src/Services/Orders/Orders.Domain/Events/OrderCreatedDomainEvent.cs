using Shared.Kernel;

namespace Orders.Domain.Events;

public record OrderCreatedDomainEvent(
    Guid OrderId,
    Guid UserId,
    decimal TotalAmount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
