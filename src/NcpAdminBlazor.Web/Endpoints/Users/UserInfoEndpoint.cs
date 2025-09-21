using FastEndpoints;

namespace NcpAdminBlazor.Web.Endpoints.Users;

sealed class UserInfoEndpoint(IMediator mediator) : Endpoint<UserInfoRequest, ResponseData<UserInfoResponse>>
{
    public override void Configure()
    {
        Get("/api/user/{userId}/profile");
        Description(x => x.WithTags("User"));
    }

    public override async Task HandleAsync(UserInfoRequest r, CancellationToken c)
    {
        var res = new UserInfoResponse(r.UserId, "myName");
        await Send.OkAsync(res.AsResponseData(), c);
    }
}

sealed class UserInfoRequest
{
    [RouteParam] public long UserId { get; set; }

    public UserInfoRequest()
    {
    }

    public UserInfoRequest(long userId) => UserId = userId;
}

sealed record UserInfoResponse(long UserId, string UserName);

sealed class UserInfoValidator : Validator<UserInfoRequest>
{
    public UserInfoValidator()
    {
        // RuleFor(x => x.Property).NotEmpty();
    }
}

sealed class UserInfoSummary : Summary<UserInfoEndpoint, UserInfoRequest>
{
    public UserInfoSummary()
    {
        Summary = "用户信息";
        Description = "";
    }
}