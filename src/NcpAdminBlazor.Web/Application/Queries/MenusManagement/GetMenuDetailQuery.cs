using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;

namespace NcpAdminBlazor.Web.Application.Queries.MenusManagement;

public sealed record GetMenuDetailQuery(MenuId MenuId)
    : IQuery<MenuDetailDto>;

public sealed class GetMenuDetailQueryValidator : AbstractValidator<GetMenuDetailQuery>
{
    public GetMenuDetailQueryValidator()
    {
        RuleFor(x => x.MenuId)
            .NotNull().WithMessage("菜单标识不能为空");
    }
}

public sealed class GetMenuDetailQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetMenuDetailQuery, MenuDetailDto>
{
    public async Task<MenuDetailDto> Handle(GetMenuDetailQuery request, CancellationToken cancellationToken)
    {
        var menu = await context.Menus
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.MenuId,
                cancellationToken)
                   ?? throw new KnownException($"未找到菜单，MenuId = {request.MenuId}");

        return new MenuDetailDto(
            menu.Id,
            menu.ParentId,
            menu.Title,
            menu.Type,
            menu.Order,
            menu.IsDisable,
            menu.Icon,
            menu.PageKey,
            menu.Path,
            menu.PermissionCode);
    }
}

public sealed record MenuDetailDto(
    MenuId MenuId,
    MenuId? ParentId,
    string Title,
    MenuType Type,
    int Order,
    bool Visible,
    string? Icon,
    string? PageKey,
    string? Path,
    string? PermissionCode);
