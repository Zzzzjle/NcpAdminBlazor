using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.RolesManagement;

public record GetRolePermissionsQuery(RoleId RoleId) : IQuery<RolePermissionsDto>;

public record RolePermissionsDto(
    RoleId RoleId,
    List<string> PermissionCodes);

public class GetRolePermissionsQueryValidator : AbstractValidator<GetRolePermissionsQuery>
{
    public GetRolePermissionsQueryValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}

public class GetRolePermissionsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetRolePermissionsQuery, RolePermissionsDto>
{
    public async Task<RolePermissionsDto> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var rolePermissions = await context.Roles
            .Where(r => r.Id == request.RoleId)
            .Select(r => new RolePermissionsDto(r.Id, r.AssignedPermissionCodes.ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        return rolePermissions ?? throw new KnownException($"未找到角色，RoleId = {request.RoleId}");
    }
}
