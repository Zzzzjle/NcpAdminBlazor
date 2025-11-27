using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
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
    private const string TestEnvAuthPolicyScheme = "TestEnvAuthPolicyScheme";
    public HttpClient AuthenticatedClient { get; private set; } = null!;

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
        //set "TestEnvAuthPolicyScheme" scheme as the default scheme and register the test handler
        s.PostConfigure<AuthenticationOptions>(options =>
        {
            options.DefaultAuthenticateScheme = TestEnvAuthPolicyScheme;
            options.DefaultChallengeScheme = TestEnvAuthPolicyScheme;
        });
        s.AddAuthentication(TestAuthHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, null)
            .AddPolicyScheme(TestEnvAuthPolicyScheme, TestEnvAuthPolicyScheme, o =>
            {
                o.ForwardDefaultSelector = ctx =>
                {
                    if (ctx.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authHeader) &&
                        authHeader.FirstOrDefault()?.Equals(TestAuthHandler.SchemeName) is true)
                    {
                        return TestAuthHandler.SchemeName;
                    }

                    return "Jwt_Or_ApiKey";
                };
            });
    }

    protected override ValueTask SetupAsync()
    {
        AuthenticatedClient = CreateClient(c =>
        {
            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName);
        });
        return ValueTask.CompletedTask;
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