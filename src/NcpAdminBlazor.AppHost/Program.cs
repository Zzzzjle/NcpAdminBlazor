var builder = DistributedApplication.CreateBuilder(args);

//Enable Docker pubilsher
builder.AddDockerComposeEnvironment("docker-env")
    .WithDashboard(dashboard =>
    {
        dashboard.WithHostPort(8080)
            .WithForwardedHeaders(enabled: true);
    });

// Add Redis infrastructure
var redis = builder.AddRedis("redis");

// Add PostgreSQL database infrastructure
var postgreSqlPassword = builder.AddParameter("postgreSql-password", secret: true);
var postgres = builder.AddPostgres("database", password: postgreSqlPassword)
    // Configure the container to store data in a volume so that it persists across instances.
    .WithDataVolume(isReadOnly: false)
    // Keep the container running between app host sessions.
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

var postgresdb = postgres.AddDatabase("PostgreSQL", "dev");

// Add RabbitMQ message queue infrastructure
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

// Add web project with infrastructure dependencies
var apiService = builder.AddProject<Projects.NcpAdminBlazor_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .PublishAsDockerComposeService((_, service) => { service.Restart = "always"; });

builder.AddProject<Projects.NcpAdminBlazor_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService).PublishAsDockerComposeService((_, service) => { service.Restart = "always"; });

await builder.Build().RunAsync();