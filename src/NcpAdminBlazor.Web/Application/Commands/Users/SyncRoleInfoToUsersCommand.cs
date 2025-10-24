using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands.Users;

public record SyncRoleInfoToUsersCommand(RoleId RoleId, string RoleName) : ICommand;

public class SyncRoleInfoToUsersCommandValidator : AbstractValidator<SyncRoleInfoToUsersCommand>
{
    public SyncRoleInfoToUsersCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("角色名称不能为空")
            .MaximumLength(50).WithMessage("角色名称不能超过50个字符");
    }
}

public class SyncRoleInfoToUsersCommandHandler(IApplicationUserRepository userRepository)
    : ICommandHandler<SyncRoleInfoToUsersCommand>
{
    public async Task Handle(SyncRoleInfoToUsersCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = request.RoleName.Trim();
        var users = await userRepository.GetByRoleIdAsync(request.RoleId, cancellationToken);
        if (users.Count == 0)
        {
            return;
        }

        foreach (var user in users)
        {
            user.UpdateRoleInfo(request.RoleId, normalizedName);
        }
    }
}
