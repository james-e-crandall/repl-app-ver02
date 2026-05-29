using FamilyTreeApi.Endpoints;
using FamilyTreeLib.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApiDocument();
builder.Services.AddEndpointsApiExplorer(); // Specifically for minimal endpoint metadata

//builder.AddSqlServerDbContext<FamilyTreeContext>(connectionName: "publisherDb");

builder.Services.AddDbContext<FamilyTreeContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("publisherDb"),
        x => x.UseHierarchyId() // <-- Add this line
    ));

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

HalflingsEndpoints.MapEndpoints(app);

app.Run();


