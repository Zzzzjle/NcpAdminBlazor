using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Domain.DomainEvents.User;

public record RoleInfoChangedDomainEvent(Role Role) : IDomainEvent;