using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.MenuAggregate;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Web.Application.Commands.RolesManagement;

namespace NcpAdminBlazor.Web.Endpoints.RolesManagement;

public sealed class UpdateRoleMenusEndpoint(IMediator mediator)
    : Endpoint<UpdateRoleMenusRequest, ResponseData>
{
    public override void Configure()
    {
        Post("/api/roles/{roleId}/menus");
        Description(d => d.WithTags("Role"));
    }

    public override async Task HandleAsync(UpdateRoleMenusRequest req, CancellationToken ct)
    {
        var command = new UpdateRoleMenusCommand(req.RoleId, req.MenuIds);
        await mediator.Send(command, ct);
        await Send.OkAsync(true.AsResponseData(), ct);
    }
}

public sealed class UpdateRoleMenusRequest
{
    [RouteParam] public required RoleId RoleId { get; init; }
    public List<MenuId> MenuIds { get; init; } = [];
}

public sealed class UpdateRoleMenusRequestValidator : AbstractValidator<UpdateRoleMenusRequest>
{
    public UpdateRoleMenusRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");

        RuleFor(x => x.MenuIds)
            .NotNull().WithMessage("菜单ID列表不能为空");
    }
}
