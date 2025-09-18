using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;

namespace NcpAdminBlazor.Domain.DomainEvents.User;

public record ApplicationUserCreatedDomainEvent(ApplicationUser User) : IDomainEvent;

public record ApplicationUserLoginDomainEvent(ApplicationUser User) : IDomainEvent;

public record ApplicationUserPasswordChangedDomainEvent(ApplicationUser User) : IDomainEvent;

public record ApplicationUserDeletedDomainEvent(ApplicationUser User) : IDomainEvent;

public record ApplicationUserRefreshTokenUpdatedDomainEvent(ApplicationUser User) : IDomainEvent;