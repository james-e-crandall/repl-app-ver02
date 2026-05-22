using ReplApi.Endpoints;
using ReplLib.Pipelines;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApiDocument();
builder.Services.AddEndpointsApiExplorer(); // Specifically for minimal endpoint metadata

builder.AddKeyedSqlServerClient(name: "publisherDb");
builder.AddKeyedSqlServerClient(name: "subscriberDb");

builder.Services.AddScoped<ReplArticle>(x => new ReplArticle
{
    ArticleName = "Halflings",
    PublicationName = "Publication1"
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Add OpenAPI 3.0 document serving middleware
    // Available at: http://localhost:<port>/swagger/v1/swagger.json
    app.UseOpenApi();

    // Add web UIs to interact with the document
    // Available at: http://localhost:<port>/swagger
    app.UseSwaggerUi(); // UseSwaggerUI Protected by if (env.IsDevelopment())
}

app.MapGet("/", () => "Hello World!");

ReplHelpEndpoints.MapEndpoints(app);

app.Run();
