using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Web.Application.Commands;
using NcpAdminBlazor.Shared.Models;

namespace NcpAdminBlazor.Web.Endpoints.UserEndpoints;

[Tags("Users")]
[HttpPut("/api/user/password")]
[Authorize]
public class ChangePasswordEndpoint(IMediator mediator) : Endpoint<ChangePasswordRequest, EmptyResponse>
{
    public override async Task HandleAsync(ChangePasswordRequest req, CancellationToken ct)
    {
        var userId = new ApplicationUserId(long.Parse(req.UserId));
        var command = new ChangePasswordCommand(userId, req.OldPassword, req.NewPassword);
        await mediator.Send(command, ct);
        
        await Send.NoContentAsync(ct);
    }
}