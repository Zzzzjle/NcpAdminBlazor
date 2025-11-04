using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.RoleAggregate;
using NcpAdminBlazor.Web.Application.Queries.RolesManagement;

namespace NcpAdminBlazor.Web.Endpoints.RolesManagement;

public sealed class RoleInfoEndpoint(IMediator mediator)
    : Endpoint<RoleInfoRequest, ResponseData<RoleInfoResponse>>
{
    public override void Configure()
    {
        Get("/api/roles/{roleId}/info");
        Description(d => d.WithTags("Role"));
    }

    public override async Task HandleAsync(RoleInfoRequest req, CancellationToken ct)
    {
        var query = new GetRoleInfoQuery(req.RoleId);
        var result = await mediator.Send(query, ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}

public sealed class RoleInfoRequest
{
    [RouteParam] public required RoleId RoleId { get; init; }
}

public sealed record RoleInfoResponse(
    RoleId RoleId,
    string Name,
    string Description,
    bool IsDisabled);

public sealed class RoleInfoRequestValidator : AbstractValidator<RoleInfoRequest>
{
    public RoleInfoRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("角色ID不能为空");
    }
}