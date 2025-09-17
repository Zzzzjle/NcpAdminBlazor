using NcpAdminBlazor.Domain.DomainEvents.User;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers;

internal class ApplicationUserPasswordChangedDomainEventHandler(ILogger<ApplicationUserPasswordChangedDomainEventHandler> logger) 
    : IDomainEventHandler<ApplicationUserPasswordChangedDomainEvent>
{
    public Task Handle(ApplicationUserPasswordChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("User password changed: UserId={UserId}, Name={Name}, ChangeTime={ChangeTime}", 
            notification.User.Id, notification.User.Name, DateTimeOffset.Now);
        
        // 这里可以添加其他业务逻辑，比如发送密码修改通知邮件、记录安全日志等
        
        return Task.CompletedTask;
    }
}