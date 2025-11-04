using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Web.Application.Queries.RolesManagement;

namespace NcpAdminBlazor.Web.Endpoints.RolesManagement;

public sealed class RolePermissionsEndpoint(IMediator mediator)
    : Endpoint<RolePermissionsRequest, ResponseData<RolePermissionsResponse>>
{
    public override void Configure()
    {
        Get("/api/roles/{roleId}/permissions");
        Description(d => d.WithTags("Role"));
    }

    public override async Task HandleAsync(RolePermissionsRequest req, CancellationToken ct)
    {
        var query = new GetRolePermissionsQuery(req.RoleId);
        var result = await mediator.Send(query, ct);
        await Send.OkAsync(new RolePermissionsResponse(result.RoleId, result.PermissionCodes).AsResponseData(), ct);
    }
}

public sealed class RolePermissionsRequest
{
    [RouteParam] public required RoleId RoleId { get; init; }
}

public sealed record RolePermissionsResponse(
    RoleId RoleId,
    List<string> PermissionCodes);

public sealed class RolePermissionsRequestValidator : AbstractValidator<RolePermissionsRequest>
{
    public RolePermissionsRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}
