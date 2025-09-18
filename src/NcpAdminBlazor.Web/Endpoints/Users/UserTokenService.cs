using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using NcpAdminBlazor.Domain.AggregatesModel.ApplicationUserAggregate;
using NcpAdminBlazor.Web.Application.Commands;
using NcpAdminBlazor.Web.Application.Queries;

namespace NcpAdminBlazor.Web.Endpoints.Users;

// accessToken管理
// https://fast-endpoints.com/docs/security#jwt-refresh-tokens
[DontRegister]
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