using System.Reflection;
using System.Text.Json;
using FastEndpoints;
using FastEndpoints.ClientGen.Kiota;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Kiota.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MudBlazor.Services;
using NcpAdminBlazor.Client.Stores;
using NcpAdminBlazor.Web.Application.IntegrationEventHandlers;
using NcpAdminBlazor.Web.Application.Queries;
using NcpAdminBlazor.Web.AspNetCore;
using NcpAdminBlazor.Web.AspNetCore.Middlewares;
using NcpAdminBlazor.Web.AspNetCore.ApiKey;
using NcpAdminBlazor.Web.AspNetCore.Permission;
using NcpAdminBlazor.Web.Clients;
using NcpAdminBlazor.Web.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prometheus;
using Refit;
using Serilog;
using StackExchange.Redis;
using SystemClock = NetCorePal.Extensions.Primitives.SystemClock;

Log.Logger = new LoggerConfiguration()
    .Enrich.WithClientIp()
    .WriteTo.Console( /*new JsonFormatter()*/)
    .CreateLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    #region SignalR

    builder.Services.AddHealthChecks();
    builder.Services.AddMvc()
        .AddNewtonsoftJson(options => { options.SerializerSettings.AddNetCorePalJsonConverters(); });
    builder.Services.AddSignalR();

    #endregion

    #region Prometheus监控

    builder.Services.AddHealthChecks().ForwardToPrometheus();
    builder.Services.AddHttpClient(Options.DefaultName)
        .UseHttpClientMetrics();

    #endregion

    // Add services to the container.

    #region 身份认证

    var redis = await ConnectionMultiplexer.ConnectAsync(builder.Configuration.GetConnectionString("Redis")!);
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => redis);
    builder.Services.AddDataProtection()
        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

    builder.Services.AddScoped<ICurrentUser, CurrentUser>();
    builder.Services.AddTransient<UserTokenService>();
    builder.Services.AddTransient<UserPermissionService>(); // 获取用户权限
    builder.Services.AddTransient<IClaimsTransformation, UserPermissionHydrator>(); // 用户权限验证

    builder.Services
        // 添加Jwt身份认证方案
        .AddAuthenticationJwtBearer(o => o.SigningKey = builder.Configuration["Auth:Jwt:TokenSigningKey"])
        .AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = "Jwt_Or_ApiKey";
            o.DefaultChallengeScheme = "Jwt_Or_ApiKey";
        })
        // 添加 ApiKey 身份认证方案
        .AddScheme<AuthenticationSchemeOptions, ApikeyAuth>(ApikeyAuth.SchemeName, null)
        // 综合认证方案（使用jwt或apikey任意一个方案请求endpoint）
        // https://fast-endpoints.com/docs/security#combined-authentication-scheme
        .AddPolicyScheme("Jwt_Or_ApiKey", "Jwt_Or_ApiKey", o =>
        {
            o.ForwardDefaultSelector = ctx =>
            {
                if ((ctx.Request.Headers.TryGetValue(ApikeyAuth.HeaderName, out var apikeyHeader) &&
                     !string.IsNullOrWhiteSpace(apikeyHeader)) ||
                    (ctx.Request.Query.TryGetValue(ApikeyAuth.HeaderName, out apikeyHeader) &&
                     !string.IsNullOrWhiteSpace(apikeyHeader)))
                {
                    return ApikeyAuth.SchemeName;
                }

                if (ctx.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authHeader) &&
                    authHeader.FirstOrDefault()?.StartsWith("Bearer ") is true)
                {
                    return JwtBearerDefaults.AuthenticationScheme;
                }

                return ApikeyAuth.SchemeName;
            };
        });

    #endregion

    #region Controller

    builder.Services.AddControllers().AddNetCorePalSystemTextJson();
    builder.Services.AddEndpointsApiExplorer();
    // builder.Services.AddSwaggerGen(c => c.AddEntityIdSchemaMap()); //强类型id swagger schema 映射
    builder.Services.SwaggerDocument(o =>
    {
        o.DocumentSettings = s => s.DocumentName = "v1"; //must match doc name below
    });

    #endregion

    #region FastEndpoints

    builder.Services.AddFastEndpoints();
    builder.Services.Configure<JsonOptions>(o =>
        o.SerializerOptions.AddNetCorePalJsonConverters());

    #endregion

    #region 公共服务

    builder.Services.AddSingleton<IClock, SystemClock>();

    #endregion

    #region 集成事件

    builder.Services.AddTransient<OrderPaidIntegrationEventHandler>();

    #endregion

    #region 模型验证器

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    builder.Services.AddKnownExceptionErrorModelInterceptor();

    #endregion

    #region 基础设施

    builder.Services.AddRepositories(typeof(ApplicationDbContext).Assembly);

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseMySql(builder.Configuration.GetConnectionString("MySql"),
            new MySqlServerVersion(new Version(9, 4, 0)));
        options.LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    });
    builder.Services.AddUnitOfWork<ApplicationDbContext>();
    builder.Services.AddRedisLocks();
    builder.Services.AddContext().AddEnvContext().AddCapContextProcessor();
    builder.Services.AddNetCorePalServiceDiscoveryClient();
    builder.Services.AddIntegrationEvents(typeof(NcpAdminBlazor.Web.Program))
        .UseCap<ApplicationDbContext>(b =>
        {
            b.RegisterServicesFromAssemblies(typeof(NcpAdminBlazor.Web.Program));
            b.AddContextIntegrationFilters();
            b.UseMySql();
        });


    builder.Services.AddCap(x =>
    {
        x.JsonSerializerOptions.AddNetCorePalJsonConverters();
        x.UseEntityFramework<ApplicationDbContext>();
        x.UseRabbitMQ(p => builder.Configuration.GetSection("RabbitMQ").Bind(p));
        x.UseDashboard(); //CAP Dashboard  path：  /cap
    });

    #endregion

    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly())
            .AddCommandLockBehavior()
            .AddKnownExceptionValidationBehavior()
            .AddUnitOfWorkBehaviors());

    #region 多环境支持与服务注册发现

    builder.Services.AddMultiEnv(envOption => envOption.ServiceName = "Abc.Template")
        .UseMicrosoftServiceDiscovery();
    builder.Services.AddConfigurationServiceEndpointProvider();
    builder.Services.AddEnvFixedConnectionChannelPool();

    #endregion

    #region 远程服务客户端配置

    var jsonSerializerSettings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };
    jsonSerializerSettings.AddNetCorePalJsonConverters();
    var ser = new NewtonsoftJsonContentSerializer(jsonSerializerSettings);
    var settings = new RefitSettings(ser);
    builder.Services.AddRefitClient<IUserServiceClient>(settings)
        .ConfigureHttpClient(client =>
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("https+http://user:8080")!))
        .AddMultiEnvMicrosoftServiceDiscovery() //多环境服务发现支持
        .AddStandardResilienceHandler(); //添加标准的重试策略

    #endregion

    #region Jobs

    builder.Services.AddHangfire(x => { x.UseRedisStorage(builder.Configuration.GetConnectionString("Redis")); });
    builder.Services.AddHangfireServer(); //hangfire dashboard  path：  /hangfire

    #endregion

    #region Blazor

    // Add MudBlazor services
    builder.Services.AddMudServices();

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveWebAssemblyComponents();

    builder.Services.AddScoped<LayoutStore>();

    #endregion


    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    app.UseKnownExceptionHandler();
    // Configure the HTTP request pipeline.

    app.UseStaticFiles();
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<CurrentUserMiddleware>();

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveWebAssemblyRenderMode()
        .AddAdditionalAssemblies(typeof(NcpAdminBlazor.Client._Imports).Assembly)
        .AllowAnonymous();

    app.MapControllers();
    app.UseFastEndpoints(c => c.Binding.UseDefaultValuesForNullableProps = false);
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerGen(); //add this
    }


    #region SignalR

    app.MapHub<NcpAdminBlazor.Web.Application.Hubs.ChatHub>("/chat");

    #endregion

    app.UseHttpMetrics();
    app.MapHealthChecks("/health");
    app.MapMetrics("/metrics"); // 通过   /metrics  访问指标
    app.UseHangfireDashboard();

    await app.GenerateApiClientsAndExitAsync(c =>
    {
        c.SwaggerDocumentName = "v1"; //must match doc name above
        c.Language = GenerationLanguage.CSharp;
        c.OutputPath = "../NcpAdminBlazor.Client/HttpClient";
        c.ClientNamespaceName = "NcpAdminBlazor.Client";
        c.ClientClassName = "ApiClient";
        // c.CreateZipArchive = true; //if you'd like a zip file as well
    });

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

#pragma warning disable S1118
namespace NcpAdminBlazor.Web
{
    public partial class Program
#pragma warning restore S1118
    {
    }
}