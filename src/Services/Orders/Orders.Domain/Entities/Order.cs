using Shared.Kernel;

namespace Orders.Domain.Entities;

public class Order : Entity
{
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private readonly List<OrderItem> _items = [];

    private Order() { }

    public static Order Create(Guid userId, IEnumerable<OrderItem> items)
    {
        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        order._items.AddRange(items);
        order.TotalAmount = order._items.Sum(i => i.UnitPrice * i.Quantity);
        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id, order.UserId, order.TotalAmount));
        return order;
    }

    public void Confirm() => Status = OrderStatus.Confirmed;
    public void Cancel() => Status = OrderStatus.Cancelled;
}

public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }

public class OrderItem : Entity
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
