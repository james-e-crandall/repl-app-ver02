using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServerDb = builder.AddSqlServer("sqlServer");

sqlServerDb
    .WithEnvironment("MSSQL_AGENT_ENABLED", "true");

var publisherDb = sqlServerDb.AddDatabase("publisherDb");
var subscriberDb = sqlServerDb.AddDatabase("subscriberDb");

var migrationService = builder.AddProject<Projects.FamilyTreeLib_MigrationService>("migrationService")
    .WithReference(publisherDb)
    .WaitFor(publisherDb)
    .WithReference(subscriberDb)
    .WaitFor(subscriberDb);

var replService = builder.AddProject<Projects.ReplService>("replService")
    .WithReference(publisherDb)
    .WaitFor(publisherDb)
    .WithReference(subscriberDb)
    .WaitFor(subscriberDb); 


builder.Build().Run();
