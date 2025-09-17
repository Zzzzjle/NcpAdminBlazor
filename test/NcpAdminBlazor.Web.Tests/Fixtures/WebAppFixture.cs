using Microsoft.AspNetCore.Hosting;
using NcpAdminBlazor.Infrastructure;

namespace NcpAdminBlazor.Web.Tests.Fixtures;

public class WebAppFixture : AppFixture<Program>
{
    private readonly TestContainerFixture _containers = new();

    protected override void ConfigureServices(IServiceCollection services)
    {
        // 服务配置完成后再处理数据库初始化
    }

    private bool _databaseInitialized = false;

    protected override async ValueTask SetupAsync()
    {
        await _containers.CreateVisualHostAsync("/");
        
        if (!_databaseInitialized)
        {
            // 确保数据库已创建和迁移
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
            _databaseInitialized = true;
        }
    }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Redis",
            _containers.RedisContainer.GetConnectionString() + ",defaultDatabase=0");
        builder.UseSetting("ConnectionStrings:MySql",
            _containers.MySqlContainer.GetConnectionString());
        builder.UseSetting("RabbitMQ:Port", _containers.RabbitMqContainer.GetMappedPublicPort(5672).ToString());
        builder.UseSetting("RabbitMQ:UserName", "guest");
        builder.UseSetting("RabbitMQ:Password", "guest");
        builder.UseSetting("RabbitMQ:VirtualHost", "/");
        builder.UseSetting("RabbitMQ:HostName", _containers.RabbitMqContainer.Hostname);
        builder.UseEnvironment("Test");
    }

    protected override ValueTask TearDownAsync()
    {
        _containers.Dispose();
        return ValueTask.CompletedTask;
    }
}