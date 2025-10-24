using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;
using NcpAdminBlazor.Web.Application.Queries.Roles;

namespace NcpAdminBlazor.Web.Application.Commands.Roles;

public record UpdateRoleInfoCommand(
    RoleId RoleId,
    string Name,
    string Description,
    int Status) : ICommand;

public class UpdateRoleInfoCommandValidator : AbstractValidator<UpdateRoleInfoCommand>
{
    public UpdateRoleInfoCommandValidator(IMediator mediator)
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("角色名称不能为空")
            .MaximumLength(50).WithMessage("角色名称不能超过50个字符")
            .MustAsync(async (command, name, cancellationToken) => !await mediator.Send(
                new CheckRoleNameConflictQuery(command.RoleId, name),
                cancellationToken))
            .WithMessage("角色名称已存在");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("角色描述不能为空")
            .MaximumLength(200).WithMessage("角色描述不能超过200个字符");

        RuleFor(x => x.Status)
            .Must(status => status is 0 or 1)
            .WithMessage("角色状态必须是0或1");
    }
}

public class UpdateRoleInfoCommandHandler(IRoleRepository roleRepository)
    : ICommandHandler<UpdateRoleInfoCommand>
{
    public async Task Handle(UpdateRoleInfoCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetAsync(request.RoleId, cancellationToken)
                   ?? throw new KnownException($"未找到角色，RoleId = {request.RoleId}");

        role.UpdateRoleInfo(request.Name, request.Description, request.Status);
    }
}