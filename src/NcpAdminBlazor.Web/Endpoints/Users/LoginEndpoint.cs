using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using NcpAdminBlazor.Web.Application.Commands;

namespace NcpAdminBlazor.Web.Endpoints.Users;

/// <summary>
/// 登录请求体
/// </summary>
/// <param name="Username">登录账号</param>
/// <param name="Password">登录密码</param>
public record LoginRequest(string Username, string Password);

public class LoginEndpoint(IMediator mediator)
    : Endpoint<LoginRequest, ResponseData<TokenResponse>>
{
    public override void Configure()
    {
        Post("/api/user/login");
        Description(x => x.WithTags("User"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var command = new LoginUserCommand(req.Username, req.Password);
        var loginResult = await mediator.Send(command, ct);
        var tokenService = Resolve<UserTokenService>();
        var envelope = await tokenService.CreateCustomToken(
            loginResult.UserId.ToString(),
            privileges: privileges =>
            {
                privileges.Claims.AddRange([
                    new Claim("ClientID", "Default"),
                    new Claim(ClaimTypes.NameIdentifier, loginResult.UserId.ToString()),
                    new Claim(ClaimTypes.Name, req.Username)
                ]);
            },
            map: tr => tr.AsResponseData()
        );
        await Send.OkAsync(envelope, ct);
    }
}

internal sealed class LoginSummary : Summary<LoginEndpoint, LoginRequest>
{
    public LoginSummary()
    {
        Summary = "用户登录";
        Description = "Description text goes here...";
    }
}