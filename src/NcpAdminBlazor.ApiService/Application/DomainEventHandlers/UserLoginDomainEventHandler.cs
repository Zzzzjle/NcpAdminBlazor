using NcpAdminBlazor.Domain.DomainEvents;

namespace NcpAdminBlazor.ApiService.Application.DomainEventHandlers;

internal class UserLoginDomainEventHandler(ILogger<UserLoginDomainEventHandler> logger) 
    : IDomainEventHandler<UserLoginDomainEvent>
{
    public Task Handle(UserLoginDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("User logged in: UserId={UserId}, Username={Username}, LoginTime={LoginTime}", 
            notification.User.Id, notification.User.Username, DateTimeOffset.Now);
        
        // 这里可以添加其他业务逻辑，比如记录登录日志、更新最后登录时间等
        
        return Task.CompletedTask;
    }
}