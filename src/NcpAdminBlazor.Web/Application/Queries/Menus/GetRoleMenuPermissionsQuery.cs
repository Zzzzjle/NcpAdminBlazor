using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.Menus;

public sealed record GetRoleMenuPermissionsQuery(IReadOnlyCollection<MenuId> MenuIds)
    : IQuery<IReadOnlyList<RoleMenuPermission>>;

public sealed class GetMenuPermissionsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetRoleMenuPermissionsQuery, IReadOnlyList<RoleMenuPermission>>
{
    public async Task<IReadOnlyList<RoleMenuPermission>> Handle(GetRoleMenuPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.MenuIds.Count == 0) return [];

        return await context.Menus
            .Where(menu => request.MenuIds.Contains(menu.Id))
            .Select(menu => new RoleMenuPermission(menu.Id, menu.PermissionCode))
            .ToListAsync(cancellationToken);
    }
}