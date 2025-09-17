using NcpAdminBlazor.Domain;
using NcpAdminBlazor.Domain.AggregatesModel.OrderAggregate;
using NcpAdminBlazor.Infrastructure;
using System.Threading;

namespace NcpAdminBlazor.Web.Application.Queries
{
    public class OrderQuery(ApplicationDbContext applicationDbContext)
    {
        public async Task<Order?> QueryOrder(OrderId orderId, CancellationToken cancellationToken)
        {
            return await applicationDbContext.Orders.FindAsync(new object[] { orderId }, cancellationToken);
        }
    }
}
