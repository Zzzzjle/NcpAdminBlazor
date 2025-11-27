using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NcpAdminBlazor.Infrastructure;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace NcpAdminBlazor.ApiService.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class WebAppFixture : AppFixture<Program>
{
    private RedisContainer _redisContainer = null!;
    private RabbitMqContainer _rabbitMqContainer = null!;
    private PostgreSqlContainer _npgSqlContainer = null!;

    protected override async ValueTask PreSetupAsync()
    {
        _redisContainer = new RedisBuilder()
            .WithCommand("--databases", "1024").Build();
        _rabbitMqContainer = new RabbitMqBuilder()
            .WithUsername("guest").WithPassword("guest").Build();
        _npgSqlContainer = new PostgreSqlBuilder()
            .WithUsername("root").WithPassword("123456")
            .WithEnvironment("TZ", "Asia/Shanghai")
            .WithDatabase("test").Build();
        await Task.WhenAll(_redisContainer.StartAsync(),
            _rabbitMqContainer.StartAsync(),
            _npgSqlContainer.StartAsync());

        await CreateDatabaseAsync(_npgSqlContainer.GetConnectionString());
    }

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        // Configure Redis connection string for Aspire
        a.UseSetting("ConnectionStrings:Redis", _redisContainer.GetConnectionString());
        a.UseSetting("ConnectionStrings:PostgreSQL", _npgSqlContainer.GetConnectionString());
        a.UseSetting("ConnectionStrings:rabbitmq",
            $"amqp://guest:guest@{_rabbitMqContainer.Hostname}:{_rabbitMqContainer.GetMappedPublicPort(5672)}/");

        a.UseEnvironment(Environments.Development);
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        // Additional service configuration for tests if needed
    }

    private static async Task CreateDatabaseAsync(string connectionString)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<WebAppFixture>());
        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        await using var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
}