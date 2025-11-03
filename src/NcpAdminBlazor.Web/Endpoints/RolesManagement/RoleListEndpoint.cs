using FastEndpoints;
using NcpAdminBlazor.Web.Application.Queries.RolesManagement;

namespace NcpAdminBlazor.Web.Endpoints.RolesManagement;

public sealed class RoleListEndpoint(IMediator mediator)
    : Endpoint<GetRoleListRequest, ResponseData<PagedData<RoleListItemDto>>>
{
    public override void Configure()
    {
        Get("/api/roles");
        Description(d => d.WithTags("Role"));
    }

    public override async Task HandleAsync(GetRoleListRequest req, CancellationToken ct)
    {
        var query = new GetRoleListQuery(
            req.Name,
            req);

        var result = await mediator.Send(query, ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}

public sealed class GetRoleListRequest : IPageRequest
{
    [QueryParam] public string? Name { get; set; }
    [QueryParam] public int PageIndex { get; set; } = 1;
    [QueryParam] public int PageSize { get; set; } = 10;
    [QueryParam] public bool CountTotal { get; set; } = true;
}

public sealed class GetRoleListRequestValidator : AbstractValidator<GetRoleListRequest>
{
    public GetRoleListRequestValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0).WithMessage("页码必须大于0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("每页条数必须在1-100之间");
    }
}