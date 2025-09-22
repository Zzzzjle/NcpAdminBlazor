using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Testcontainers.MySql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using System.Net.Http.Headers;

namespace NcpAdminBlazor.Web.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class WebAppFixture : AppFixture<Program>
{
    private RedisContainer _redisContainer = null!;
    private RabbitMqContainer _rabbitMqContainer = null!;
    private MySqlContainer _mySqlContainer = null!;

    public HttpClient DefaultClient { get; private set; } = null!;
    public HttpClient AuthenticatedClient { get; private set; } = null!;
    

    protected override async ValueTask PreSetupAsync()
    {
        _redisContainer = new RedisBuilder()
            .WithCommand("--databases", "1024").Build();
        _rabbitMqContainer = new RabbitMqBuilder()
            .WithUsername("guest").WithPassword("guest").Build();
        _mySqlContainer = new MySqlBuilder()
            .WithUsername("root").WithPassword("123456")
            .WithEnvironment("TZ", "Asia/Shanghai")
            .WithDatabase("test").Build();
        await Task.WhenAll(_redisContainer.StartAsync(),
            _rabbitMqContainer.StartAsync(),
            _mySqlContainer.StartAsync());
    }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Redis"] = _redisContainer.GetConnectionString() + ",defaultDatabase=0",
                ["ConnectionStrings:MySql"] = _mySqlContainer.GetConnectionString(),
                ["RabbitMQ:HostName"] = _rabbitMqContainer.Hostname,
                ["RabbitMQ:Port"] = _rabbitMqContainer.GetMappedPublicPort(5672).ToString(),
                ["RabbitMQ:Username"] = "guest",
                ["RabbitMQ:Password"] = "guest",
                ["RabbitMQ:VirtualHost"] = "/",
                ["Auth:ApiKey"] = "test-api-key"
            });
        });
        
        builder.UseEnvironment("Development");
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // 添加伪造认证方案
        services.AddAuthentication(TestAuthConstants.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthConstants.SchemeName, options => { });
    }

    protected override ValueTask SetupAsync()
    {
        DefaultClient = CreateClient();
        AuthenticatedClient = CreateClient(
            c =>
            {
                // 测试认证：通过 ApiKey 方案携带固定密钥
                c.DefaultRequestHeaders.Add("x-api-key", "test-api-key");
                // 保留测试自定义方案头（若将来策略切换可回退使用）
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthConstants.SchemeName);
            });
        return ValueTask.CompletedTask;
    }

    protected override ValueTask TearDownAsync()
    {
        //await _container.DisposeAsync();
        //NOTE: there's no need to dispose the container here as it will be automatically disposed by testcontainers pkg when the test run finishes.
        //      this is especially true if this AppFixture is used by many test-classes with WAF caching enabled.
        //      so, in general - containers don't need to be explicitly disposed, unless you disable WAF caching.
        return base.TearDownAsync();
    }
}