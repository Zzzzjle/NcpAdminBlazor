using System.Linq;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.Shared.Auth;

namespace NcpAdminBlazor.Web.Application.Commands.Roles;

public record UpdateRolePermissionsCommand(
    RoleId RoleId,
    IEnumerable<RoleMenuPermission> MenuPermissions) : ICommand;

public class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    public UpdateRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.MenuPermissions)
            .NotNull().WithMessage("权限列表不能为空");

        RuleForEach(x => x.MenuPermissions)
            .NotNull().WithMessage("权限项不能为空");
    }
}

public class UpdateRolePermissionsCommandHandler(IRoleRepository roleRepository)
    : ICommandHandler<UpdateRolePermissionsCommand>
{
    public async Task Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetAsync(request.RoleId, cancellationToken)
                   ?? throw new KnownException($"未找到角色，RoleId = {request.RoleId}");

        var menuPermissions = (request.MenuPermissions ?? Enumerable.Empty<RoleMenuPermission>())
            .Select(permission => new RoleMenuPermission(permission.MenuId, permission.PermissionCode))
            .ToList();

        role.UpdatePermissions(menuPermissions);
    }
}