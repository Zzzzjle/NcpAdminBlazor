using Microsoft.EntityFrameworkCore;
using NcpAdminBlazor.Infrastructure;
using NcpAdminBlazor.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ApiDbInitializer>();

builder.AddServiceDefaults();

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(c =>
    c.RegisterServicesFromAssemblies(assembly));

builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"), sqlOptions =>
        sqlOptions.MigrationsAssembly(assembly.FullName)
    ));
builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

var host = builder.Build();
await host.RunAsync();