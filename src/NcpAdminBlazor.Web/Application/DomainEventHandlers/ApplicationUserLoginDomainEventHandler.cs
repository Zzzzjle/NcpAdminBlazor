using NcpAdminBlazor.Domain.DomainEvents.User;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers;

internal class ApplicationUserLoginDomainEventHandler(ILogger<ApplicationUserLoginDomainEventHandler> logger) 
    : IDomainEventHandler<ApplicationUserLoginDomainEvent>
{
    public Task Handle(ApplicationUserLoginDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("User logged in: UserId={UserId}, Username={Username}, LoginTime={LoginTime}", 
            notification.User.Id, notification.User.Username, DateTimeOffset.Now);
        
        // 这里可以添加其他业务逻辑，比如记录登录日志、更新最后登录时间等
        
        return Task.CompletedTask;
    }
}