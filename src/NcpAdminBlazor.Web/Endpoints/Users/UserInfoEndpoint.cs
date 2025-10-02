using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Web.Application.Queries;

namespace NcpAdminBlazor.Web.Endpoints.Users;

public sealed class UserInfoEndpoint(IMediator mediator) : Endpoint<UserInfoRequest, ResponseData<UserInfoDto>>
{
    public override void Configure()
    {
        Get("/api/user/{userId}/profile");
        Description(x => x.WithTags("User"));
    }

    public override async Task HandleAsync(UserInfoRequest r, CancellationToken ct)
    {
        var userId = new ApplicationUserId(r.UserId);
        var dto = await mediator.Send(new GetUserInfoQuery(userId), ct);
        await Send.OkAsync(dto.AsResponseData(), ct);
    }
}

public sealed class UserInfoRequest
{
    [RouteParam] public long UserId { get; set; }

    public UserInfoRequest()
    {
    }

    public UserInfoRequest(long userId) => UserId = userId;
}

sealed class UserInfoValidator : AbstractValidator<UserInfoRequest>
{
    public UserInfoValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);
    }
}

sealed class UserInfoSummary : Summary<UserInfoEndpoint, UserInfoRequest>
{
    public UserInfoSummary()
    {
        Summary = "获取指定用户信息";
        Description = "根据用户ID获取详细资料，包括角色与权限";
    }
}