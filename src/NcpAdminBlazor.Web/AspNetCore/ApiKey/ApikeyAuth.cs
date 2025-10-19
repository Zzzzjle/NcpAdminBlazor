using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace NcpAdminBlazor.Web.AspNetCore.ApiKey;

sealed class ApikeyAuth(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration config)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    internal const string SchemeName = "ApiKey";
    internal const string HeaderName = "x-api-key";

    readonly string _apiKey = config["Auth:ApiKey"] ??
                              throw new InvalidOperationException("Api key not set in appsettings.json");

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // 从请求头获取apikey
        Request.Headers.TryGetValue(HeaderName, out var extractedApiKey);
        // 若请求头不存在则从查询参数获取
        if (string.IsNullOrWhiteSpace(extractedApiKey))
            Request.Query.TryGetValue(ApikeyAuth.HeaderName, out extractedApiKey);


        // 通过apikey初始化当前用户
        var user = await InitLoginUserAsync(extractedApiKey);

        if (!IsPublicEndpoint() && user.UserId == 0)
            return AuthenticateResult.Fail("Invalid API credentials!");

        // 传递身份信息
        var ticket = CreateTicket(user);
        return AuthenticateResult.Success(ticket);
    }

    private async Task<LoginUser> InitLoginUserAsync(StringValues extractedApiKey)
    {
        // 临时代码：后续改成通过apikey从缓存加载用户信息
        var loginUser = new LoginUser();
        if (!extractedApiKey.Equals(_apiKey))
        {
            loginUser.UserId = 0;
            loginUser.UserName = "匿名访客";
        }
        else
        {
            loginUser.UserId = 123;
            loginUser.UserName = "登录用户";
        }

        await Task.CompletedTask;
        return loginUser;
    }

    private AuthenticationTicket CreateTicket(LoginUser user)
    {
        ClaimsIdentity identity;
        if (user.UserId == 0) // 匿名访客
        {
            identity = new ClaimsIdentity(
                claims:
                [
                    new Claim(ClaimTypes.NameIdentifier, "0"),
                    new Claim(ClaimTypes.Role, "guest"),
                ],
                authenticationType: Scheme.Name);
        }
        else // 已登录访客
        {
            identity = new ClaimsIdentity(
                claims:
                [
                    new Claim("ClientID", "Default"),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, "admin"), // 登录访客（临时代码）
                    new Claim("permissions", "pms1"),
                    new Claim("permissions", "pms2")
                    // ...
                ],
                authenticationType: Scheme.Name);
        }

        var principal = new GenericPrincipal(identity, roles: null);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return ticket;
    }

    bool IsPublicEndpoint()
        => Context.GetEndpoint()?.Metadata.OfType<AllowAnonymousAttribute>().Any() is null or true;
}

public class LoginUser
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}