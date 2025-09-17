using NcpAdminBlazor.Domain.DomainEvents;
using NcpAdminBlazor.Web.Application.IntegrationEventHandlers;
using NetCorePal.Extensions.DistributedTransactions;

namespace NcpAdminBlazor.Web.Application.IntegrationEventConverters;

public class OrderPaidIntegrationEventConverter
    : IIntegrationEventConverter<OrderPaidDomainEvent, OrderPaidIntegrationEvent>
{
    public OrderPaidIntegrationEvent Convert(OrderPaidDomainEvent domainEvent)
    {
        return new OrderPaidIntegrationEvent(domainEvent.Order.Id);
    }
}