using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.RolesManagement;

public record CheckRoleNameConflictQuery(RoleId RoleId, string Name) : IQuery<bool>;

public class CheckRoleNameConflictQueryValidator : AbstractValidator<CheckRoleNameConflictQuery>
{
    public CheckRoleNameConflictQueryValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("角色名称不能为空");
    }
}

public class CheckRoleNameConflictQueryHandler(ApplicationDbContext context)
    : IQueryHandler<CheckRoleNameConflictQuery, bool>
{
    public async Task<bool> Handle(CheckRoleNameConflictQuery request, CancellationToken cancellationToken)
    {
        return await context.Roles
            .AnyAsync(role => role.Id != request.RoleId && role.Name == request.Name, cancellationToken);
    }
}