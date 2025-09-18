using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NcpAdminBlazor.Shared.EndpointsDtos.UserEndpoints;
using NetCorePal.Extensions.Jwt;
using NcpAdminBlazor.Web.Application.Commands;

namespace NcpAdminBlazor.Web.Endpoints.UserEndpoints;

[Tags("Users")]
[HttpPost("/api/user/login")]
[AllowAnonymous]
public class LoginEndpoint(IMediator mediator, IJwtProvider jwtProvider) : Endpoint<LoginRequest, ResponseData<LoginResponse>>
{
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var command = new LoginUserCommand(req.LoginName, req.Password);
        var loginResult = await mediator.Send(command, ct);
        
        // 生成JWT Token
        var claims = new List<Claim>
        {
            new("userId", loginResult.UserId.ToString()),
            new("name", loginResult.Name),
            new("email", loginResult.Email),
            new("realName", loginResult.RealName)
        };
        claims.AddRange(loginResult.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // 添加角色声明

        // 添加权限声明
        claims.AddRange(loginResult.Permissions.Select(permission => new Claim("permission", permission)));

        var jwt = await jwtProvider.GenerateJwtToken(
            new JwtData("netcorepal", "netcorepal",
                claims,
                DateTime.Now, DateTime.Now.AddDays(1)), ct);
        
        var response = new LoginResponse
        {
            UserId = loginResult.UserId.ToString(),
            Name = loginResult.Name,
            Email = loginResult.Email,
            RealName = loginResult.RealName,
            Roles = loginResult.Roles,
            Permissions = loginResult.Permissions,
            Token = jwt
        };
        
        await Send.OkAsync(response.AsResponseData(), cancellation: ct);
    }
}