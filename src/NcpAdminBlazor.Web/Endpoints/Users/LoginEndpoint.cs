using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Web.Application.Commands;
using NcpAdminBlazor.Web.Application.Queries;

namespace NcpAdminBlazor.Web.Endpoints.Users;

/// <summary>
/// 登录请求体
/// </summary>
/// <param name="Username">登录账号</param>
/// <param name="Password">登录密码</param>
public record LoginRequest(string Username, string Password);

public class LoginEndpoint(IMediator mediator) : Endpoint<LoginRequest, TokenResponse>
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

        Response = await CreateTokenWith<UserTokenService>(loginResult.UserId.ToString(), u =>
        {
            u.Claims.AddRange(
            [
                new Claim("ClientID", "Default"),
                new Claim(ClaimTypes.NameIdentifier, loginResult.UserId.ToString()),
                new Claim(ClaimTypes.Name, req.Username)
            ]);
        });
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

// accessToken管理
// https://fast-endpoints.com/docs/security#jwt-refresh-tokens
public class UserTokenService : RefreshTokenService<TokenRequest, TokenResponse>
{
    private readonly IMediator _mediator;

    public UserTokenService(IConfiguration config, IMediator mediator)
    {
        _mediator = mediator;
        Setup(o =>
        {
            o.TokenSigningKey = config["Auth:Jwt:TokenSigningKey"];
            o.AccessTokenValidity = TimeSpan.FromMinutes(5);
            o.RefreshTokenValidity = TimeSpan.FromHours(4);
            o.Issuer = "my-first-ncp";
            o.Audience = "my-first-ncp";

            // token刷新路由（提交 TokenRequest 对象到此路由）
            o.Endpoint("/api/user/refresh-token",
                ep =>
                {
                    ep.Tags("User");
                    ep.Summary(s => s.Summary = "刷新令牌");
                });
        });
    }

    // 保存当前用户刷新token
    public override async Task PersistTokenAsync(TokenResponse rsp)
    {
        // 将刷新令牌持久化到用户聚合
        if (!ApplicationUserId.TryParse(rsp.UserId, out var userId))
        {
            throw new KnownException($"无效的用户ID: {rsp.UserId}");
        }

        var command = new UpdateUserRefreshTokenCommand(userId, rsp.RefreshToken, rsp.RefreshExpiry);
        await _mediator.Send(command);
    }

    /// <summary>
    /// 验证refreshToken是否有效
    /// </summary>
    /// <param name="req"></param>
    public override async Task RefreshRequestValidationAsync(TokenRequest req)
    {
        if (!ApplicationUserId.TryParse(req.UserId, out var userId))
        {
            AddError(r => r.UserId, "无效的用户ID");
            return;
        }

        if (!await _mediator.Send(new CheckRefreshTokenIsValidQuery(userId, req.RefreshToken)))
        {
            AddError(r => r.RefreshToken, "无效的刷新令牌");
        }
    }

    /// <summary>
    /// refreshToken验证有效后重新生成accessToken
    /// </summary>
    /// <param name="request"></param>
    /// <param name="privileges"></param>
    /// <exception cref="KnownException"></exception>
    public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
    {
        if (!ApplicationUserId.TryParse(request.UserId, out var userId))
        {
            throw new KnownException($"无效的用户ID: {request.UserId}");
        }

        var username = await _mediator.Send(new GetUsernameByIdQuery(userId));

        privileges.Claims.AddRange(
        [
            new Claim("ClientID", "Default"),
            new Claim(ClaimTypes.NameIdentifier, request.UserId),
            new Claim(ClaimTypes.Name, username)
        ]);
    }
}

// JWT 令牌注销
// 使用提供的抽象中间件类可以轻松实现令牌吊销。重写 JwtTokenIsValidAsync() 方法，
// 并在检查数据库或吊销令牌缓存后，如果提供的令牌不再有效，则返回 false。
public class MyBlacklistChecker(RequestDelegate next) : JwtRevocationMiddleware(next)
{
    protected override Task<bool> JwtTokenIsValidAsync(string jwtToken, CancellationToken ct)
    {
        // return true if the supplied token is still valid
        return Task.FromResult(true);
    }
}