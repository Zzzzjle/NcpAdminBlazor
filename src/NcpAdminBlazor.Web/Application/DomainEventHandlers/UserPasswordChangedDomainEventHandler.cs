using NcpAdminBlazor.Domain.DomainEvents;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers;

internal class UserPasswordChangedDomainEventHandler(ILogger<UserPasswordChangedDomainEventHandler> logger) 
    : IDomainEventHandler<UserPasswordChangedDomainEvent>
{
    public Task Handle(UserPasswordChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("User password changed: UserId={UserId}, Username={Username}, ChangeTime={ChangeTime}", 
            notification.User.Id, notification.User.Username, DateTimeOffset.Now);
        
        // 这里可以添加其他业务逻辑，比如发送密码修改通知邮件、记录安全日志等
        
        return Task.CompletedTask;
    }
}