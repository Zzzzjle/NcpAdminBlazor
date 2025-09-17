using NcpAdminBlazor.Domain.AggregatesModel.OrderAggregate;

namespace NcpAdminBlazor.Domain.DomainEvents;

public record OrderPaidDomainEvent(Order Order) : IDomainEvent;