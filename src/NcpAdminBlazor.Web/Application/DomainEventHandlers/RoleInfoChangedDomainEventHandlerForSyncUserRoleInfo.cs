using NcpAdminBlazor.Domain.DomainEvents.User;
using NcpAdminBlazor.Web.Application.Commands.Roles;
using NcpAdminBlazor.Web.Application.Commands.Users;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers;

internal class RoleInfoChangedDomainEventHandlerForSyncUserRoleInfo(IMediator mediator)
    : IDomainEventHandler<RoleInfoChangedDomainEvent>
{
    public async Task Handle(RoleInfoChangedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var role = domainEvent.Role;
        var command = new SyncRoleInfoToUsersCommand(role.Id, role.Name);
        await mediator.Send(command, cancellationToken);
    }
}
