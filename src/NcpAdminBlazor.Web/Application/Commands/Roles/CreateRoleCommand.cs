using System.Linq;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.Web.Application.Queries.Roles;

namespace NcpAdminBlazor.Web.Application.Commands.Roles;

public record CreateRoleCommand(
    string Name,
    string Description,
    IEnumerable<RoleMenuPermission> MenuPermissions) : ICommand<RoleId>;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(IMediator mediator)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("角色名称不能为空")
            .MaximumLength(50).WithMessage("角色名称不能超过50个字符")
            .MustAsync(async (name, cancellationToken) =>
                !await mediator.Send(new CheckRoleExistsByNameQuery(name), cancellationToken))
            .WithMessage("角色名称已存在");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("角色描述不能为空")
            .MaximumLength(200).WithMessage("角色描述不能超过200个字符");

        RuleFor(x => x.MenuPermissions)
            .NotNull().WithMessage("角色权限列表不能为空");
    }
}

public class CreateRoleCommandHandler(IRoleRepository roleRepository)
    : ICommandHandler<CreateRoleCommand, RoleId>
{
    public async Task<RoleId> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var menuPermissions = (request.MenuPermissions ?? Enumerable.Empty<RoleMenuPermission>())
            .Select(permission => new RoleMenuPermission(permission.MenuId, permission.PermissionCode))
            .ToList();

        var role = new Role(request.Name, request.Description, menuPermissions);
        await roleRepository.AddAsync(role, cancellationToken);
        return role.Id;
    }
}