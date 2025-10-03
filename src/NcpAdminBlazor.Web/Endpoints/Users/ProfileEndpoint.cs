using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Web.Application.Queries;
using NcpAdminBlazor.Web.AspNetCore;

namespace NcpAdminBlazor.Web.Endpoints.Users;

public class ProfileEndpoint(IMediator mediator, ICurrentUser currentUser) : EndpointWithoutRequest<ResponseData<UserInfoDto>>
{
    public override void Configure()
    {
        Get("/api/user/profile");
        Description(x => x.WithTags("User"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = new ApplicationUserId(currentUser.UserId);
        var dto = await mediator.Send(new GetUserInfoQuery(userId), ct);
        await Send.OkAsync(dto.AsResponseData(), ct);
    }
}

sealed class ProfileSummary : Summary<ProfileEndpoint>
{
    public ProfileSummary()
    {
        Summary = "获取当前用户信息";
        Description = "返回当前登录用户的详细资料";
        
    }
}