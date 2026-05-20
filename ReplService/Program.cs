using ReplService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddReplicationPipeline();
builder.AddKeyedSqlServerClient(name: "publisherDb");
builder.AddKeyedSqlServerClient(name: "subscriberDb");

var host = builder.Build();
host.Run();
