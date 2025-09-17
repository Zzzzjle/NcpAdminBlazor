using NcpAdminBlazor.Domain.AggregatesModel.OrderAggregate;

namespace NcpAdminBlazor.Domain.DomainEvents
{
    public record OrderCreatedDomainEvent(Order Order) : IDomainEvent;
}
