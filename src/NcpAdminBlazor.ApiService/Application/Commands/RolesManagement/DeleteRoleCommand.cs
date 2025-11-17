using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure.Repositories;

namespace NcpAdminBlazor.ApiService.Application.Commands.RolesManagement;

public record DeleteRoleCommand(RoleId RoleId) : ICommand;

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}

public class DeleteRoleCommandHandler(IRoleRepository roleRepository)
    : ICommandHandler<DeleteRoleCommand>
{
    public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetAsync(request.RoleId, cancellationToken)
                   ?? throw new KnownException($"未找到角色，RoleId = {request.RoleId}");

        role.Delete();
    }
}