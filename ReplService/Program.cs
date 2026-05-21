using ReplLib.Options;
using ReplService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddReplicationPipeline();
builder.AddKeyedSqlServerClient(name: "publisherDb");
builder.AddKeyedSqlServerClient(name: "subscriberDb");
builder.Services.Configure<ReplDbSecrets>(builder.Configuration);
var host = builder.Build();

host.Run();

