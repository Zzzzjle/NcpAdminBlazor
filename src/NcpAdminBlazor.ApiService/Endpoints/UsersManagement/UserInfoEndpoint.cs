using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;
using NcpAdminBlazor.ApiService.Application.Queries.UsersManagement;

namespace NcpAdminBlazor.ApiService.Endpoints.UsersManagement;

public sealed class UserInfoEndpoint(IMediator mediator) : Endpoint<UserInfoRequest, ResponseData<UserInfoDto>>
{
    public override void Configure()
    {
        Get("/api/user/{userId}/profile");
        Description(x => x.WithTags("User"));
    }

    public override async Task HandleAsync(UserInfoRequest r, CancellationToken ct)
    {
        var dto = await mediator.Send(new GetUserInfoQuery(r.UserId), ct);
        await Send.OkAsync(dto.AsResponseData(), ct);
    }
}

public sealed class UserInfoRequest
{
    [RouteParam] public required UserId UserId { get; set; }
}

public sealed class UserInfoSummary : Summary<UserInfoEndpoint, UserInfoRequest>
{
    public UserInfoSummary()
    {
        Summary = "获取指定用户信息";
        Description = "根据用户ID获取详细资料，包括角色与权限";
    }
}