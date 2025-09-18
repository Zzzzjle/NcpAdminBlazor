using NcpAdminBlazor.Domain.DomainEvents.User;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers;

internal class ApplicationUserDeletedDomainEventHandler(ILogger<ApplicationUserDeletedDomainEventHandler> logger) 
    : IDomainEventHandler<ApplicationUserDeletedDomainEvent>
{
    public Task Handle(ApplicationUserDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("User deleted: UserId={UserId}, Username={Username}, DeleteTime={DeleteTime}", 
            notification.User.Id, notification.User.Username, DateTimeOffset.Now);
        
        // 这里可以添加其他业务逻辑，比如清理用户相关数据、发送账户注销确认等
        
        return Task.CompletedTask;
    }
}