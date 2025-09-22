using FastEndpoints;
using NcpAdminBlazor.Shared.Auth;

namespace NcpAdminBlazor.Web.Endpoints.Users;

public sealed class DeleteUserEndpoint : Endpoint<DeleteUserRequest, ResponseData>
{
    public override void Configure()
    {
        Delete("/api/user/{userId}/delete");
        Description(x => x.WithTags("User")); // 路由分组
        Permissions(AppPermissions.Keys.System_Users_Delete); // 需要的权限
    }

    public override async Task HandleAsync(DeleteUserRequest r, CancellationToken c)
    {
        await Send.OkAsync(true.AsResponseData(), c);
    }
}

public sealed class DeleteUserRequest
{
    [RouteParam] public long UserId { get; set; }
}

public sealed class DeleteUserValidator : Validator<DeleteUserRequest>
{
    public DeleteUserValidator()
    {
    }
}

public sealed class DeleteUserSummary : Summary<DeleteUserEndpoint, DeleteUserRequest>
{
    public DeleteUserSummary()
    {
        Summary = "删除用户";
        Description = "";
    }
}