using NcpAdminBlazor.Domain.DomainEvents.User;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers;

internal class ApplicationUserCreatedDomainEventHandler(ILogger<ApplicationUserCreatedDomainEventHandler> logger) 
    : IDomainEventHandler<ApplicationUserCreatedDomainEvent>
{
    public Task Handle(ApplicationUserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("User created: UserId={UserId}, Username={Username}, Email={Email}", 
            notification.User.Id, notification.User.Username, notification.User.Email);
        
        // 这里可以添加其他业务逻辑，比如发送欢迎邮件
        
        return Task.CompletedTask;
    }
}