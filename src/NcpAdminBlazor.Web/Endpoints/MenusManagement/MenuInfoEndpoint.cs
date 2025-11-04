using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Web.Application.Queries.MenusManagement;

namespace NcpAdminBlazor.Web.Endpoints.MenusManagement;

public sealed class MenuInfoEndpoint(IMediator mediator)
    : Endpoint<MenuInfoRequest, ResponseData<MenuInfoResponse>>
{
    public override void Configure()
    {
        Get("/api/menus/{menuId}/info");
        Description(d => d.WithTags("Menu"));
    }

    public override async Task HandleAsync(MenuInfoRequest req, CancellationToken ct)
    {
        var query = new GetMenuInfoQuery(req.MenuId);
        var menu = await mediator.Send(query, ct);

        await Send.OkAsync(menu.AsResponseData(), ct);
    }
}

public sealed class MenuInfoRequest
{
    [RouteParam] public required MenuId MenuId { get; init; }
}

public sealed record MenuInfoResponse(
    MenuId MenuId,
    MenuId ParentId,
    string Title,
    MenuType Type,
    int Order,
    bool IsDisabled,
    string Icon,
    string PageKey,
    string Path,
    string PermissionCode);

public sealed class MenuInfoRequestValidator : AbstractValidator<MenuInfoRequest>
{
    public MenuInfoRequestValidator()
    {
        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("菜单标识不能为空");
    }
}