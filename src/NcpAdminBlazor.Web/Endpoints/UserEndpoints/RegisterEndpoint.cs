using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NcpAdminBlazor.Web.Application.Commands;
using NcpAdminBlazor.Shared.Models;

namespace NcpAdminBlazor.Web.Endpoints.UserEndpoints;

[Tags("Users")]
[HttpPost("/api/user/register")]
[AllowAnonymous]
public class RegisterEndpoint(IMediator mediator) : Endpoint<RegisterRequest, ResponseData<RegisterResponse>>
{
    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var command = new RegisterUserCommand(req.Name, req.Email, req.Password, req.Phone, req.RealName);
        var userId = await mediator.Send(command, ct);
        
        var response = new RegisterResponse { UserId = userId.ToString() };
        await Send.OkAsync(response.AsResponseData(), ct);
    }
}