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

// Add RabbitMQ message queue infrastructure
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

// Add PostgreSQL database infrastructure
var postgresPassword = builder.AddParameter("postgres-password", secret: true);
var postgres = builder.AddPostgres("database", password: postgresPassword)
    // Configure the container to store data in a volume so that it persists across instances.
    .WithDataVolume(isReadOnly: false)
    // Keep the container running between app host sessions.
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

var postgresDb = postgres.AddDatabase("PostgreSQL", "dev");

var migrationService = builder.AddProject<Projects.NcpAdminBlazor_MigrationService>("migration")
    .WithReference(postgresDb)
    .WaitFor(postgresDb);

// Add web project with infrastructure dependencies
var apiService = builder.AddProject<Projects.NcpAdminBlazor_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(postgresDb)
    .WaitForCompletion(migrationService)
    .PublishAsDockerComposeService((_, service) => { service.Restart = "always"; });

builder.AddProject<Projects.NcpAdminBlazor_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService).PublishAsDockerComposeService((_, service) => { service.Restart = "always"; });

await builder.Build().RunAsync();