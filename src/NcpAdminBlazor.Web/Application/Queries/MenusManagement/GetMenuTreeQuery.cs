using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.MenusManagement;

public sealed record GetMenuTreeQuery(bool IncludeInvisible = false)
    : IQuery<IReadOnlyList<MenuTreeNodeDto>>;

public sealed class GetMenuTreeQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetMenuTreeQuery, IReadOnlyList<MenuTreeNodeDto>>
{
    public async Task<IReadOnlyList<MenuTreeNodeDto>> Handle(GetMenuTreeQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Menu> query = context.Menus
            .AsNoTracking();

        if (!request.IncludeInvisible)
        {
            query = query.Where(menu => menu.IsDisable);
        }

        var menus = await query
            .OrderBy(menu => menu.ParentId)
            .ThenBy(menu => menu.Order)
            .ToListAsync(cancellationToken);

        if (menus.Count == 0)
        {
            return Array.Empty<MenuTreeNodeDto>();
        }

        var nodeLookup = menus.ToDictionary(
            menu => menu.Id,
            menu => new MenuTreeNodeDto(
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

        var roots = new List<MenuTreeNodeDto>();
        foreach (var menu in menus)
        {
            var node = nodeLookup[menu.Id];
            if (menu.ParentId is { } parentId && nodeLookup.TryGetValue(parentId, out var parentNode))
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

public sealed record MenuTreeNodeDto(
    MenuId MenuId,
    MenuId? ParentId,
    string Title,
    MenuType Type,
    int Order,
    bool Visible,
    string? Icon,
    string? PageKey,
    string? Path,
    string? PermissionCode)
{
    public List<MenuTreeNodeDto> Children { get; } = new();
}
