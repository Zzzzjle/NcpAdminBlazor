using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.Domain.DomainEvents;

public record UserCreatedDomainEvent(User User) : IDomainEvent;

public record UserInfoUpdatedDomainEvent(User User) : IDomainEvent;

public record UserLoginDomainEvent(User User) : IDomainEvent;

public record UserPasswordChangedDomainEvent(User User) : IDomainEvent;

public record UserDeletedDomainEvent(User User) : IDomainEvent;