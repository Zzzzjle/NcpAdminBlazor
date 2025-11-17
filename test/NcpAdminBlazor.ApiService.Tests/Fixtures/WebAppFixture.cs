using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.MySql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace NcpAdminBlazor.ApiService.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class WebAppFixture : AppFixture<Program>
{
    private RedisContainer _redisContainer = null!;
    private RabbitMqContainer _rabbitMqContainer = null!;
    private MySqlContainer _mySqlContainer = null!;

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
        // Configure Redis connection string for Aspire
        a.UseSetting("ConnectionStrings:Redis", _redisContainer.GetConnectionString());
        a.UseSetting("ConnectionStrings:MySql", _mySqlContainer.GetConnectionString());
        a.UseSetting("ConnectionStrings:rabbitmq",
            $"amqp://guest:guest@{_rabbitMqContainer.Hostname}:{_rabbitMqContainer.GetMappedPublicPort(5672)}/");
        
        a.UseEnvironment("Development");
    }
    
    protected override void ConfigureServices(IServiceCollection s)
    {
        // Additional service configuration for tests if needed
    }
}