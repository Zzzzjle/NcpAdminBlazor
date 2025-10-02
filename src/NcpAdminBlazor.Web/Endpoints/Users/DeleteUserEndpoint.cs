using FastEndpoints;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Shared.Auth;
using NcpAdminBlazor.Web.Application.Commands;

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
        var command = new DeleteUserCommand(new ApplicationUserId(r.UserId));
        await mediator.Send(command, ct);
        await Send.OkAsync(true.AsResponseData(), ct);
    }
}

public sealed class DeleteUserRequest
{
    [RouteParam] public long UserId { get; set; }
}

public sealed class DeleteUserValidator : AbstractValidator<DeleteUserRequest>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("用户ID必须大于0");
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