using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NcpAdminBlazor.Shared.EndpointsDtos.UserEndpoints;
using NcpAdminBlazor.Web.Application.Commands;

namespace NcpAdminBlazor.Web.Endpoints.UserEndpoints;

[Tags("Users")]
[HttpPut("/api/user/password")]
[Authorize]
public class ChangePasswordEndpoint(IMediator mediator) : Endpoint<ChangePasswordRequest, EmptyResponse>
{
    public override async Task HandleAsync(ChangePasswordRequest req, CancellationToken ct)
    {
        var command = new ChangePasswordCommand(req.UserId, req.OldPassword, req.NewPassword);
        await mediator.Send(command, ct);
        
        await Send.NoContentAsync(ct);
    }
}