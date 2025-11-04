using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Web.Endpoints.MenusManagement;

namespace NcpAdminBlazor.Web.Application.Queries.MenusManagement;

public sealed record GetMenuInfoQuery(MenuId MenuId)
    : IQuery<MenuInfoResponse>;

public sealed class GetMenuInfoQueryValidator : AbstractValidator<GetMenuInfoQuery>
{
    public GetMenuInfoQueryValidator()
    {
        RuleFor(x => x.MenuId)
            .NotNull().WithMessage("菜单标识不能为空");
    }
}

public sealed class GetMenuInfoQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetMenuInfoQuery, MenuInfoResponse>
{
    public async Task<MenuInfoResponse> Handle(GetMenuInfoQuery request, CancellationToken cancellationToken)
    {
        var menu = await context.Menus
                       .AsNoTracking()
                       .FirstOrDefaultAsync(
                           x => x.Id == request.MenuId,
                           cancellationToken)
                   ?? throw new KnownException($"未找到菜单，MenuId = {request.MenuId}");

        return new MenuInfoResponse(
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