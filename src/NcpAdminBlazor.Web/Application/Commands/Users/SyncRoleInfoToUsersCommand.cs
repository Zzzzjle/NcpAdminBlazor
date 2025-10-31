using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands.Users;

public record SyncRoleInfoToUsersCommand(
    ApplicationUserId UserId,
    RoleId RoleId,
    string RoleName,
    bool IsDisable) : ICommand;

public class SyncRoleInfoToUsersCommandValidator : AbstractValidator<SyncRoleInfoToUsersCommand>
{
    public SyncRoleInfoToUsersCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("角色名称不能为空");
    }
}

public class SyncRoleInfoToUsersCommandHandler(IApplicationUserRepository userRepository)
    : ICommandHandler<SyncRoleInfoToUsersCommand>
{
    public async Task Handle(SyncRoleInfoToUsersCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken)
                   ?? throw new KnownException($"未找到用户，UserId = {request.UserId}");

        user.SyncRoleInfo(request.RoleId, request.RoleName, request.IsDisable);
    }
}