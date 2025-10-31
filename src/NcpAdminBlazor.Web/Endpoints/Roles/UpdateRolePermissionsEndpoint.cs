using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Web.Application.Commands.Roles;
using NcpAdminBlazor.Web.Application.Queries.Menus;

namespace NcpAdminBlazor.Web.Endpoints.Roles;

public sealed class UpdateRolePermissionsEndpoint(IMediator mediator)
    : Endpoint<UpdateRolePermissionsRequest, ResponseData>
{
    public override void Configure()
    {
        Post("/api/roles/{roleId}/permissions");
        Description(d => d.WithTags("Role"));
    }

    public override async Task HandleAsync(UpdateRolePermissionsRequest req, CancellationToken ct)
    {
        var menuIds = req.MenuIds ?? [];
        var menuPermissions = await mediator.Send(new GetRoleMenuPermissionsQuery(menuIds), ct);
        await mediator.Send(new UpdateRolePermissionsCommand(req.RoleId, menuPermissions), ct);
        await Send.OkAsync(true.AsResponseData(), ct);
    }
}

public sealed class UpdateRolePermissionsRequest
{
    [RouteParam] public required RoleId RoleId { get; init; }
    public List<MenuId> MenuIds { get; init; } = [];
}

public sealed class UpdateRolePermissionsRequestValidator : AbstractValidator<UpdateRolePermissionsRequest>
{
    public UpdateRolePermissionsRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.MenuIds)
            .NotNull().WithMessage("菜单ID列表不能为空");
    }
}