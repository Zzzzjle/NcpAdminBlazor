using System.Linq;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Domain.DomainEvents;
using NcpAdminBlazor.Web.Application.Commands.Users;
using NcpAdminBlazor.Web.Application.Queries.Users;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers;

internal class RolePermissionChangedDomainEventHandlerForSyncUserPermissions(IMediator mediator)
    : IDomainEventHandler<RolePermissionChangedDomainEvent>
{
    public async Task Handle(RolePermissionChangedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var role = domainEvent.Role;
        var affectedUserIds = await mediator.Send(new GetUserIdsByRoleIdQuery(role.Id), cancellationToken);

        var menuPermissions = role.MenuPermissions
            .Select(p => new UserMenuPermission(p.MenuId, role.Id, p.PermissionCode))
            .ToList();

        foreach (var userId in affectedUserIds)
        {
            await mediator.Send(new SyncRolePermissionsToUsersCommand(userId, role.Id, menuPermissions),
                cancellationToken);
        }
    }
}