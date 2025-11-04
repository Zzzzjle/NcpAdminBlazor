using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Web.Application.Queries.MenusManagement;

namespace NcpAdminBlazor.Web.Endpoints.MenusManagement;

public sealed class MenuTreeEndpoint(IMediator mediator)
    : Endpoint<MenuTreeRequest, ResponseData<List<MenuTreeNodeResponse>>>
{
    public override void Configure()
    {
        Get("/api/menus/tree");
        Description(d => d.WithTags("Menu"));
    }

    public override async Task HandleAsync(MenuTreeRequest req, CancellationToken ct)
    {
        var query = new GetMenuTreeQuery();
        var tree = await mediator.Send(query, ct);
        await Send.OkAsync(tree.ToList().AsResponseData(), ct);
    }
}

public sealed class MenuTreeRequest
{
}

public sealed class MenuTreeRequestValidator : AbstractValidator<MenuTreeRequest>
{
    public MenuTreeRequestValidator()
    {
    }
}

public sealed record MenuTreeNodeResponse(
    MenuId MenuId,
    MenuId ParentId,
    string Title,
    MenuType Type,
    int Order,
    bool IsDisabled,
    string Icon,
    string PageKey,
    string Path,
    string PermissionCode)
{
    public List<MenuTreeNodeResponse> Children { get; init; } = [];
}