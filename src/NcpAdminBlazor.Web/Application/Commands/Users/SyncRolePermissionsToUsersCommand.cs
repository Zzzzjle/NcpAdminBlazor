using System.Collections.Generic;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands.Users;

public record SyncRolePermissionsToUsersCommand(
    ApplicationUserId UserId,
    RoleId RoleId,
    IEnumerable<UserMenuPermission> MenuPermissions) : ICommand;

public class SyncRolePermissionsToUsersCommandValidator : AbstractValidator<SyncRolePermissionsToUsersCommand>
{
    public SyncRolePermissionsToUsersCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.MenuPermissions)
            .NotNull().WithMessage("权限列表不能为空");
    }
}

public class SyncRolePermissionsToUsersCommandHandler(IApplicationUserRepository userRepository)
    : ICommandHandler<SyncRolePermissionsToUsersCommand>
{
    public async Task Handle(SyncRolePermissionsToUsersCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken)
                   ?? throw new KnownException($"未找到用户，UserId = {request.UserId}");

        user.SyncRolePermissions(request.RoleId, request.MenuPermissions);
    }
}
