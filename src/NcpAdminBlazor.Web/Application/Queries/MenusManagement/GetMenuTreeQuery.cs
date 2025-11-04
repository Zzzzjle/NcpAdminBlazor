using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Web.Endpoints.MenusManagement;

namespace NcpAdminBlazor.Web.Application.Queries.MenusManagement;

public sealed record GetMenuTreeQuery
    : IQuery<IReadOnlyList<MenuTreeNodeResponse>>;

public sealed class GetMenuTreeQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetMenuTreeQuery, IReadOnlyList<MenuTreeNodeResponse>>
{
    public async Task<IReadOnlyList<MenuTreeNodeResponse>> Handle(GetMenuTreeQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Menus
            .AsNoTracking();

        var menus = await query
            .OrderBy(menu => menu.ParentId)
            .ThenBy(menu => menu.Order)
            .ToListAsync(cancellationToken);

        if (menus.Count == 0) return [];

        var nodeLookup = menus.ToDictionary(
            menu => menu.Id,
            menu => new MenuTreeNodeResponse(
                menu.Id,
                menu.ParentId,
                menu.Title,
                menu.Type,
                menu.Order,
                menu.IsDisable,
                menu.Icon,
                menu.PageKey,
                menu.Path,
                menu.PermissionCode));

        var roots = new List<MenuTreeNodeResponse>();
        foreach (var menu in menus)
        {
            var node = nodeLookup[menu.Id];
            if (nodeLookup.TryGetValue(menu.ParentId, out var parentNode))
            {
                parentNode.Children.Add(node);
            }
            else
            {
                roots.Add(node);
            }
        }

        return roots
            .OrderBy(node => node.Order)
            .ThenBy(node => node.Title, StringComparer.Ordinal)
            .ToList();
    }
}