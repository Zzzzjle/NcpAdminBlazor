using NcpAdminBlazor.Domain.DomainEvents.User;
using NcpAdminBlazor.Web.Application.Commands.Roles;
using NcpAdminBlazor.Web.Application.Commands.Users;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers;

internal class RolePermissionChangedDomainEventHandlerForSyncUserPermissions(IMediator mediator)
    : IDomainEventHandler<RolePermissionChangedDomainEvent>
{
    public async Task Handle(RolePermissionChangedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var role = domainEvent.Role;
        var permissionCodes = role.Permissions.Select(p => p.PermissionCode).ToList();
        var command = new SyncRolePermissionsToUsersCommand(role.Id, permissionCodes);
        await mediator.Send(command, cancellationToken);
    }
}
