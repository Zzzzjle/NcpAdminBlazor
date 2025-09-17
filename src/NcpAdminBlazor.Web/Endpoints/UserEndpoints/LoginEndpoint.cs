using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Jwt;
using NcpAdminBlazor.Web.Application.Commands;
using NcpAdminBlazor.Shared.Models;

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
        
        // 添加角色声明
        foreach (var role in loginResult.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        // 添加权限声明
        foreach (var permission in loginResult.Permissions)
        {
            claims.Add(new Claim("permission", permission));
        }
        
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