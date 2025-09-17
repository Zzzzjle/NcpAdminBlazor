using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Domain.DomainEvents.User;

public record RolePermissionChangedDomainEvent(Role Role) : IDomainEvent;
