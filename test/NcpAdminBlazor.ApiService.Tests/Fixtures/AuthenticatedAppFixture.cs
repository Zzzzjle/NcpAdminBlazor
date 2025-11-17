using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NcpAdminBlazor.ApiService.Tests.Fixtures;

public class AuthenticatedAppFixture : WebAppFixture
{
    public HttpClient AuthenticatedClient { get; private set; } = null!;

    protected override void ConfigureServices(IServiceCollection s)
    {
        // 移除所有已注册的 IAuthenticationService 和相关方案
        // 确保我们伪造的认证是唯一生效的
        // 移除所有已注册的 AuthenticationHandler
        s.RemoveAll<IAuthenticationHandlerProvider>();

        s.PostConfigure<AuthenticationOptions>(options =>
        {
            options.DefaultAuthenticateScheme = TestAuthConstants.SchemeName;
            options.DefaultChallengeScheme = TestAuthConstants.SchemeName;
        });
        // 添加伪造认证方案
        s.AddAuthentication(TestAuthConstants.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthConstants.SchemeName, _ => { });
    }

    protected override ValueTask SetupAsync()
    {
        AuthenticatedClient = CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthConstants.SchemeName);
        });
        return ValueTask.CompletedTask;
    }
}