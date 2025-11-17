using NcpAdminBlazor.Domain.DomainEvents;
using NcpAdminBlazor.ApiService.Application.Commands;
using MediatR;
using NetCorePal.Extensions.Domain;

namespace NcpAdminBlazor.ApiService.Application.DomainEventHandlers
{
    internal class OrderCreatedDomainEventHandler(IMediator mediator) : IDomainEventHandler<OrderCreatedDomainEvent>
    {
        public Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            return mediator.Send(new DeliverGoodsCommand(notification.Order.Id), cancellationToken);
        }
    }
}