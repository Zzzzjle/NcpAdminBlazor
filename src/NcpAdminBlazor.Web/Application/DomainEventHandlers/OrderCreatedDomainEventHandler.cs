using NcpAdminBlazor.Domain.DomainEvents;
using NcpAdminBlazor.Web.Application.Commands;
using MediatR;
using NetCorePal.Extensions.Domain;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers
{
    internal class OrderCreatedDomainEventHandler(IMediator mediator) : IDomainEventHandler<OrderCreatedDomainEvent>
    {
        public Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            return mediator.Send(new DeliverGoodsCommand(notification.Order.Id), cancellationToken);
        }
    }
}