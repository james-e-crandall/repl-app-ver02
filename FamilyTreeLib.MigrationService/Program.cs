using FamilyTreeLib.Data;
using FamilyTreeLib.MigrationService;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.AddServiceDefaults();

builder.Services.AddDbContextPool<FamilyTreeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("publisherDb"), sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("FamilyTreeLib.MigrationService");
        sqlOptions.UseHierarchyId();
    } 
    ));
builder.EnrichSqlServerDbContext<FamilyTreeContext>();

var host = builder.Build();
host.Run();
