using NcpAdminBlazor.Domain.DomainEvents;
using NcpAdminBlazor.Web.Application.Commands.Users;
using NcpAdminBlazor.Web.Application.Queries.Users;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers;

internal class RoleInfoChangedDomainEventHandlerForSyncUserRoleInfo(IMediator mediator)
    : IDomainEventHandler<RoleInfoChangedDomainEvent>
{
    public async Task Handle(RoleInfoChangedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var role = domainEvent.Role;
        var affectedUserIds = await mediator.Send(new GetUserIdsByRoleIdQuery(role.Id), cancellationToken);
        foreach (var userId in affectedUserIds)
        {
            await mediator.Send(new SyncRoleInfoToUsersCommand(userId, role.Id, role.Name, role.IsDisabled),
                cancellationToken);
        }
    }
}