using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
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

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        a.UseSetting("ConnectionStrings:Redis", _redisContainer.GetConnectionString());
        a.UseSetting("ConnectionStrings:MySql", _mySqlContainer.GetConnectionString());
        a.UseSetting("RabbitMQ:Port", _rabbitMqContainer.GetMappedPublicPort(5672).ToString());
        a.UseSetting("RabbitMQ:UserName", "guest");
        a.UseSetting("RabbitMQ:Password", "guest");
        a.UseSetting("RabbitMQ:VirtualHost", "/");
        a.UseSetting("RabbitMQ:HostName", _rabbitMqContainer.Hostname);
        a.UseEnvironment("Development");
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        // 添加伪造认证方案
        s.AddAuthentication(TestAuthConstants.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthConstants.SchemeName, _ => { });
    }

    protected override ValueTask SetupAsync()
    {
        DefaultClient = CreateClient();
        AuthenticatedClient = CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthConstants.SchemeName);
        });
        return ValueTask.CompletedTask;
    }
}