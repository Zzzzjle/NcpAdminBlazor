using NcpAdminBlazor.Domain.DomainEvents;
using NcpAdminBlazor.ApiService.Application.IntegrationEventHandlers;
using NetCorePal.Extensions.DistributedTransactions;

namespace NcpAdminBlazor.ApiService.Application.IntegrationEventConverters;

public class OrderPaidIntegrationEventConverter
    : IIntegrationEventConverter<OrderPaidDomainEvent, OrderPaidIntegrationEvent>
{
    public OrderPaidIntegrationEvent Convert(OrderPaidDomainEvent domainEvent)
    {
        return new OrderPaidIntegrationEvent(domainEvent.Order.Id);
    }
}