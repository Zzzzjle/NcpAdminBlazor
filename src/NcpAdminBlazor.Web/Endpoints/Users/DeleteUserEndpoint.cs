using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Shared.Auth;
using NcpAdminBlazor.Web.Application.Commands;
using NcpAdminBlazor.Web.Application.Commands.Users;

namespace NcpAdminBlazor.Web.Endpoints.Users;

public sealed class DeleteUserEndpoint(IMediator mediator) : Endpoint<DeleteUserRequest, ResponseData>
{
    public override void Configure()
    {
        Delete("/api/user/{userId}/delete");
        Description(x => x.WithTags("User")); // 路由分组
        Permissions(AppPermissions.Keys.System_Users_Delete); // 需要的权限
    }

    public override async Task HandleAsync(DeleteUserRequest r, CancellationToken ct)
    {
        await mediator.Send(new DeleteUserCommand(r.UserId), ct);
        await Send.OkAsync(true.AsResponseData(), ct);
    }
}

public sealed class DeleteUserRequest
{
    [RouteParam] public required ApplicationUserId UserId { get; set; }
}

public sealed class DeleteUserSummary : Summary<DeleteUserEndpoint, DeleteUserRequest>
{
    public DeleteUserSummary()
    {
        Summary = "删除用户";
        Description = "";
    }
}