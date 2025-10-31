using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Web.Application.Queries.Roles;

namespace NcpAdminBlazor.Web.Endpoints.Roles;

public sealed class RolePermissionsEndpoint(IMediator mediator)
    : Endpoint<RolePermissionsRequest, ResponseData<RolePermissionsDto>>
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
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}

public sealed class RolePermissionsRequest
{
    [RouteParam] public required RoleId RoleId { get; init; }
}

public sealed class RolePermissionsRequestValidator : AbstractValidator<RolePermissionsRequest>
{
    public RolePermissionsRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}
