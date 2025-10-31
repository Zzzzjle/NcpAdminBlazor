using NcpAdminBlazor.Domain.DomainEvents;

namespace NcpAdminBlazor.Web.Application.DomainEventHandlers;

internal class UserCreatedDomainEventHandler(ILogger<UserCreatedDomainEventHandler> logger) 
    : IDomainEventHandler<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("User created: UserId={UserId}, Username={Username}, Email={Email}", 
            notification.User.Id, notification.User.Username, notification.User.Email);
        
        // 这里可以添加其他业务逻辑，比如发送欢迎邮件
        
        return Task.CompletedTask;
    }
}