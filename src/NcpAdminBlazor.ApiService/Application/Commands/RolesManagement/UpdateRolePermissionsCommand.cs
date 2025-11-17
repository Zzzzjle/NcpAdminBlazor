using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.ApiService.Application.Commands.RolesManagement;

public record UpdateRolePermissionsCommand(
    RoleId RoleId,
    List<string> PermissionCodes) : ICommand;

public class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    public UpdateRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.PermissionCodes)
            .NotNull().WithMessage("权限代码列表不能为空");
    }
}

public class UpdateRolePermissionsCommandHandler(IRoleRepository roleRepository)
    : ICommandHandler<UpdateRolePermissionsCommand>
{
    public async Task Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetAsync(request.RoleId, cancellationToken)
                   ?? throw new KnownException($"未找到角色，RoleId = {request.RoleId}");
        
        role.UpdatePermissions(request.PermissionCodes);
    }
}
