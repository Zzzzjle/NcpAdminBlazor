using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace NcpAdminBlazor.Infrastructure;

public class DesignTimeApplicationDbContextFactory: IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddMediatR(c =>
            c.RegisterServicesFromAssemblies(typeof(DesignTimeApplicationDbContextFactory).Assembly));
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            // change connectionstring if you want to run command “dotnet ef database update”
            options.UseMySql("Server=localhost;User ID=root;Password=123456;Database=demo;SslMode=none;",
                new MySqlServerVersion(new Version(8, 0, 34)),
                b =>
                {
                    b.MigrationsAssembly(typeof(DesignTimeApplicationDbContextFactory).Assembly.FullName);
                    b.UseMicrosoftJson();
                });
        });
        var provider = services.BuildServiceProvider();
        var dbContext = provider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return dbContext;
    }
}