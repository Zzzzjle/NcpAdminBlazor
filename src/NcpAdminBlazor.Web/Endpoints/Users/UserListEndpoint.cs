using FastEndpoints;
using NcpAdminBlazor.Web.Application.Queries.Users;

namespace NcpAdminBlazor.Web.Endpoints.Users;

public sealed class UserListEndpoint(IMediator mediator)
    : Endpoint<GetUserListRequest, ResponseData<PagedData<UserListItemDto>>>
{
    public override void Configure()
    {
        Get("/api/user/list");
        Description(d => d.WithTags("User"));
    }

    public override async Task HandleAsync(GetUserListRequest req, CancellationToken ct)
    {
        var query = new GetUserListQuery(
            req.Username,
            req.Email,
            req.Phone,
            req.RealName,
            req.PageIndex,
            req.PageSize,
            req.CountTotal
        );

        var result = await mediator.Send(query, ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}

public sealed class GetUserListRequest : IPageRequest
{
    [QueryParam] public string? Username { get; set; }
    [QueryParam] public string? Email { get; set; }
    [QueryParam] public string? Phone { get; set; }
    [QueryParam] public string? RealName { get; set; }
    [QueryParam] public int PageIndex { get; set; } = 1;
    [QueryParam] public int PageSize { get; set; } = 10;
    [QueryParam] public bool CountTotal { get; set; } = true;
}

public sealed class GetUserListRequestValidator : AbstractValidator<GetUserListRequest>
{
    public GetUserListRequestValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0)
            .WithMessage("页码必须大于0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("每页条数必须在1-100之间");
    }
}

public sealed class UserListSummary : Summary<UserListEndpoint, GetUserListRequest>
{
    public UserListSummary()
    {
        Summary = "获取用户列表";
        Description = "按条件分页查询用户";
    }
}