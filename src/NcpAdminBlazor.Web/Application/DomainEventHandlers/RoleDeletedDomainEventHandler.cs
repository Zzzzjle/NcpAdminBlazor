using NcpAdminBlazor.Domain.DomainEvents;
using NcpAdminBlazor.Web.Application.Commands.Users;
using NcpAdminBlazor.Web.Application.Queries.Users;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers;

public class RoleDeletedDomainEventHandler(IMediator mediator) : IDomainEventHandler<RoleDeletedDomainEvent>
{
    public async Task Handle(RoleDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var role = notification.Role;
        var affectedUserIds = await mediator.Send(new GetUserIdsByRoleIdQuery(role.Id), cancellationToken);
        foreach (var userId in affectedUserIds)
        {
            await mediator.Send(new RemoveUserRoleCommand(userId, role.Id), cancellationToken);
        }
    }
}