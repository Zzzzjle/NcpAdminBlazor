using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Domain.DomainEvents;

public record RolePermissionChangedDomainEvent(Role Role) : IDomainEvent;

public record RoleDeletedDomainEvent(Role Role) : IDomainEvent;

public record RoleInfoChangedDomainEvent(Role Role) : IDomainEvent;

public record RoleMenusChangedDomainEvent(Role Role) : IDomainEvent;