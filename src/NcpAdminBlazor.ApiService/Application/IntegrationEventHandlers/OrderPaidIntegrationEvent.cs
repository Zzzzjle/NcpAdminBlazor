using NcpAdminBlazor.Domain.AggregatesModel.OrderAggregate;

namespace NcpAdminBlazor.ApiService.Application.IntegrationEventHandlers
{
    public record OrderPaidIntegrationEvent(OrderId OrderId);
}
