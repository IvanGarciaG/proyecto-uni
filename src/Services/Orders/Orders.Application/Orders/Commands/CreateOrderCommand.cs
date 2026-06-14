using MassTransit;
using MediatR;
using Orders.Domain.Entities;
using Shared.Contracts.Events;
using Shared.Kernel;

namespace Orders.Application.Orders.Commands;

public record CreateOrderItemRequest(Guid ProductId, int Quantity, decimal UnitPrice);

public record CreateOrderCommand(Guid UserId, IReadOnlyList<CreateOrderItemRequest> Items)
    : IRequest<Result<Guid>>;

public class CreateOrderCommandHandler(IOrderRepository repository, IPublishEndpoint bus)
    : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand cmd, CancellationToken ct)
    {
        var items = cmd.Items.Select(i => new OrderItem
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        });

        var order = Order.Create(cmd.UserId, items);

        await repository.AddAsync(order, ct);
        await repository.SaveChangesAsync(ct);

        await bus.Publish(new OrderCreatedEvent(
            order.Id,
            order.UserId,
            order.TotalAmount,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, i.UnitPrice)).ToList()
        ), ct);

        return Result<Guid>.Success(order.Id);
    }
}
