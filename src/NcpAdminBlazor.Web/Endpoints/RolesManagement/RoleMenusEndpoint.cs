using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Web.Application.Queries.RolesManagement;

namespace NcpAdminBlazor.Web.Endpoints.RolesManagement;

public sealed class RoleMenusEndpoint(IMediator mediator)
    : Endpoint<RoleMenusRequest, ResponseData<RoleMenusResponse>>
{
    public override void Configure()
    {
        Get("/api/roles/{roleId}/menus");
        Description(d => d.WithTags("Role"));
    }

    public override async Task HandleAsync(RoleMenusRequest req, CancellationToken ct)
    {
        var query = new GetRoleMenusQuery(req.RoleId);
        var result = await mediator.Send(query, ct);
        await Send.OkAsync(new RoleMenusResponse(result.RoleId, result.MenuIds).AsResponseData(), ct);
    }
}

public sealed class RoleMenusRequest
{
    [RouteParam] public required RoleId RoleId { get; init; }
}

public sealed record RoleMenusResponse(
    RoleId RoleId,
    List<MenuId> MenuIds);

public sealed class RoleMenusRequestValidator : AbstractValidator<RoleMenusRequest>
{
    public RoleMenusRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}
