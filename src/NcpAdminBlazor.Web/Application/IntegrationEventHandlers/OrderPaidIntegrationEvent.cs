using NcpAdminBlazor.Domain.AggregatesModel.OrderAggregate;

namespace NcpAdminBlazor.Web.Application.IntegrationEventHandlers
{
    public record OrderPaidIntegrationEvent(OrderId OrderId);
}
