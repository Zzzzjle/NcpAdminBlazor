var builder = DistributedApplication.CreateBuilder(args);

// Add Redis infrastructure
var redis = builder.AddRedis("Redis");

// Add MySQL database infrastructure
var mysql = builder.AddMySql("Database")
    .WithPhpMyAdmin()
    .AddDatabase("MySql", "dev");

// Add RabbitMQ message queue infrastructure
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

// Add web project with infrastructure dependencies
var apiService = builder.AddProject<Projects.NcpAdminBlazor_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(mysql)
    .WaitFor(mysql)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.NcpAdminBlazor_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

await builder.Build().RunAsync();