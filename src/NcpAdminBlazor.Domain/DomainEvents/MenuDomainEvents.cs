using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Domain.DomainEvents;

public record MenuCreatedDomainEvent(Menu Menu) : IDomainEvent;

public record MenuUpdatedDomainEvent(Menu Menu) : IDomainEvent;

public record MenuDeletedDomainEvent(Menu Menu) : IDomainEvent;

public record MenuReorderedDomainEvent(Menu Menu) : IDomainEvent;

public record MenuPermissionsChangedDomainEvent(Menu Menu) : IDomainEvent;

public record MenuVisibilityChangedDomainEvent(Menu Menu) : IDomainEvent;
