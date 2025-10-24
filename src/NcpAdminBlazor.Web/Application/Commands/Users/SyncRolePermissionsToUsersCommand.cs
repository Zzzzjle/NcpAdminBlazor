using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands.Users;

public record SyncRolePermissionsToUsersCommand(
    RoleId RoleId,
    IEnumerable<string> PermissionCodes) : ICommand;

public class SyncRolePermissionsToUsersCommandValidator : AbstractValidator<SyncRolePermissionsToUsersCommand>
{
    public SyncRolePermissionsToUsersCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}

public class SyncRolePermissionsToUsersCommandHandler(IApplicationUserRepository userRepository)
    : ICommandHandler<SyncRolePermissionsToUsersCommand>
{
    public async Task Handle(SyncRolePermissionsToUsersCommand request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetByRoleIdAsync(request.RoleId, cancellationToken);
        if (users.Count == 0)
        {
            return;
        }

        foreach (var user in users)
        {
            var permissions = request.PermissionCodes
                .Select(code => new ApplicationUserPermission(code, request.RoleId))
                .ToList();
            user.UpdateRolePermissions(request.RoleId, permissions);
        }
    }
}