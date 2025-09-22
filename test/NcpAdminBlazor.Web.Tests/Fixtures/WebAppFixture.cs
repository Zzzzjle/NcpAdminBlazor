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
    private static int _index;
    private static int _portIndex = 8080;
    private readonly int _instanceIndex;
    private int _listeningPort;

    public HttpClient DefaultClient { get; private set; } = null!;
    public HttpClient AuthenticatedClient { get; private set; } = null!;

    public WebAppFixture()
    {
        _instanceIndex = _index++;
    }

    protected override async ValueTask PreSetupAsync()
    {
        _redisContainer = new RedisBuilder()
            .WithCommand("--databases", "1024").Build();
        _rabbitMqContainer = new RabbitMqBuilder()
            .WithUsername("guest").WithPassword("guest").Build();
        _mySqlContainer = new MySqlBuilder()
            .WithUsername("root").WithPassword("123456")
            .WithEnvironment("TZ", "Asia/Shanghai")
            .WithDatabase("mysql").Build();
        await Task.WhenAll(_redisContainer.StartAsync(),
            _rabbitMqContainer.StartAsync(),
            _mySqlContainer.StartAsync());
        await CreateVisualHostAsync("v" + _instanceIndex);
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
                ["RabbitMQ:VirtualHost"] = "/"
            });
        });

        _listeningPort = _portIndex++;
        var url = $"http://*:{_listeningPort}";
        builder.UseSetting(WebHostDefaults.ServerUrlsKey, url);
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
        var clientOptions = new ClientOptions { BaseAddress = new Uri($"http://localhost:{_listeningPort}") };
        DefaultClient = CreateClient(clientOptions);
        AuthenticatedClient = CreateClient(
            c =>
            {
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthConstants.SchemeName);
            }, clientOptions);
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

    private async Task CreateVisualHostAsync(string visualHost)
    {
        await _rabbitMqContainer.ExecAsync(["rabbitmqctl", "add_vhost", visualHost]);
        await _rabbitMqContainer.ExecAsync([
            "rabbitmqctl", "set_permissions", "-p", visualHost, "guest", ".*", ".*", ".*"
        ]);
    }
}