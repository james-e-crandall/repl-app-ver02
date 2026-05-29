using ReplLib.Options;
using ReplLib.Pipelines;
using ReplService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddReplicationPipeline();
builder.AddKeyedSqlServerClient(name: "publisherDb");
builder.AddKeyedSqlServerClient(name: "subscriberDb");
builder.Services.Configure<ReplDbSecrets>(builder.Configuration);

builder.Services.AddScoped<ReplArticle>(x => new ReplArticle
{
    ArticleName = "Halflings",
    PublicationName = "Publication1"
});

builder.Services.AddScoped<ReplArticle>(x => new ReplArticle
{
    ArticleName = "Orcs",
    PublicationName = "Publication1"
});

var host = builder.Build();

host.Run();

