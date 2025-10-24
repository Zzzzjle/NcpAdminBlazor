using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.Web.Application.Queries.Roles;

namespace NcpAdminBlazor.Web.Application.Commands.Roles;

public record CreateRoleCommand(
    string Name,
    string Description,
    int Status,
    IEnumerable<string> PermissionCodes) : ICommand<RoleId>;

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

        RuleFor(x => x.Status)
            .Must(status => status is 0 or 1)
            .WithMessage("角色状态必须是0或1");
    }
}

public class CreateRoleCommandHandler(IRoleRepository roleRepository)
    : ICommandHandler<CreateRoleCommand, RoleId>
{
    public async Task<RoleId> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var permissions = request.PermissionCodes
            .Select(code => new RolePermission(code))
            .ToList();
        var role = new Role(request.Name, request.Description, permissions, request.Status);
        await roleRepository.AddAsync(role, cancellationToken);
        return role.Id;
    }
}