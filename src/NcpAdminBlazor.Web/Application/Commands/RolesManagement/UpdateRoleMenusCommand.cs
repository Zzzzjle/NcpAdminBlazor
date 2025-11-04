using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.Web.Application.Commands.RolesManagement;

public record UpdateRoleMenusCommand(
    RoleId RoleId,
    List<MenuId> MenuIds) : ICommand;

public class UpdateRoleMenusCommandValidator : AbstractValidator<UpdateRoleMenusCommand>
{
    public UpdateRoleMenusCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.MenuIds)
            .NotNull().WithMessage("菜单ID列表不能为空");
    }
}

public class UpdateRoleMenusCommandHandler(IRoleRepository roleRepository)
    : ICommandHandler<UpdateRoleMenusCommand>
{
    public async Task Handle(UpdateRoleMenusCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetAsync(request.RoleId, cancellationToken)
                   ?? throw new KnownException($"未找到角色，RoleId = {request.RoleId}");
        
        role.UpdateMenus(request.MenuIds);
    }
}
