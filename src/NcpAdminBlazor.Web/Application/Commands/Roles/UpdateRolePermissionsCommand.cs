using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.Shared.Auth;

namespace NcpAdminBlazor.Web.Application.Commands.Roles;

public record UpdateRolePermissionsCommand(
    RoleId RoleId,
    IEnumerable<string> PermissionCodes) : ICommand;

public class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    public UpdateRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleForEach(x => x.PermissionCodes)
            .Must(code => AppPermissions.GetAllPermissionKeys().Contains(code))
            .WithMessage("存在无效的权限代码");
    }
}

public class UpdateRolePermissionsCommandHandler(IRoleRepository roleRepository)
    : ICommandHandler<UpdateRolePermissionsCommand>
{
    public async Task Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetAsync(request.RoleId, cancellationToken)
                   ?? throw new KnownException($"未找到角色，RoleId = {request.RoleId}");

        var permissions = request.PermissionCodes
            .Select(code => new RolePermission(code))
            .ToList();

        role.UpdateRolePermissions(permissions);
    }
}