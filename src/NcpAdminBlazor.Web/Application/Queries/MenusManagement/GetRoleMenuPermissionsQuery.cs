using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.MenusManagement;

public sealed record GetRoleMenuPermissionsQuery(IReadOnlyCollection<MenuId> MenuIds)
    : IQuery<IReadOnlyList<MenuPermissionDto>>;

public sealed class GetRoleMenuPermissionsQueryValidator : AbstractValidator<GetRoleMenuPermissionsQuery>
{
    public GetRoleMenuPermissionsQueryValidator()
    {
        RuleFor(x => x.MenuIds)
            .NotNull();
    }
}

public sealed class GetMenuPermissionsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetRoleMenuPermissionsQuery, IReadOnlyList<MenuPermissionDto>>
{
    public async Task<IReadOnlyList<MenuPermissionDto>> Handle(GetRoleMenuPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.MenuIds.Count == 0) return [];

        return await context.Menus
            .Where(menu => request.MenuIds.Contains(menu.Id))
            .Select(menu => new MenuPermissionDto(menu.Id, menu.PermissionCode))
            .ToListAsync(cancellationToken);
    }
}

public sealed record MenuPermissionDto(MenuId MenuId, string? PermissionCode);