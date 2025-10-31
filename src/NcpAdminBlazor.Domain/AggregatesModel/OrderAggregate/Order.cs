using NcpAdminBlazor.Domain.DomainEvents;

namespace NcpAdminBlazor.Domain.AggregatesModel.OrderAggregate;

public partial record OrderId : IGuidStronglyTypedId;

/// <summary>
/// 聚合根
/// </summary>
public class Order : Entity<OrderId>, IAggregateRoot
{
    /// <summary>
    /// 受保护的默认构造函数，用以作为EF Core的反射入口
    /// </summary>
    protected Order()
    {
    }

    public Order(string name, int count)
    {
        Name = name;
        Count = count;
        AddDomainEvent(new OrderCreatedDomainEvent(this));
    }

    public bool Paid { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int Count { get; private set; }

    public RowVersion RowVersion { get; private set; } = new();

    public UpdateTime UpdateTime { get; private set; } = new(DateTimeOffset.UtcNow);

    public void OrderPaid()
    {
        if (Paid)
        {
            throw new KnownException("Order has been paid");
        }

        Paid = true;
        AddDomainEvent(new OrderPaidDomainEvent(this));
    }
}