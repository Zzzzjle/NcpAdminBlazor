using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Infrastructure;

namespace NcpAdminBlazor.Web.Application.Queries.RolesManagement;

public record GetRoleMenusQuery(RoleId RoleId) : IQuery<RoleMenusDto>;

public record RoleMenusDto(
    RoleId RoleId,
    List<MenuId> MenuIds);

public class GetRoleMenusQueryValidator : AbstractValidator<GetRoleMenusQuery>
{
    public GetRoleMenusQueryValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}

public class GetRoleMenusQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetRoleMenusQuery, RoleMenusDto>
{
    public async Task<RoleMenusDto> Handle(GetRoleMenusQuery request, CancellationToken cancellationToken)
    {
        var roleMenus = await context.Roles
            .Where(r => r.Id == request.RoleId)
            .Select(r => new RoleMenusDto(r.Id, r.AssignedMenuIds.ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        return roleMenus ?? throw new KnownException($"未找到角色，RoleId = {request.RoleId}");
    }
}
