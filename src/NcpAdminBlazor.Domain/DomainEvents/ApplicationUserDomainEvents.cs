using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Domain.DomainEvents;

public record UserCreatedDomainEvent(ApplicationUser User) : IDomainEvent;

public record UserLoginDomainEvent(ApplicationUser User) : IDomainEvent;

public record UserPasswordChangedDomainEvent(ApplicationUser User) : IDomainEvent;

public record UserDeletedDomainEvent(ApplicationUser User) : IDomainEvent;

public record UserMenuPermissionsChanged(ApplicationUser User) : IDomainEvent;