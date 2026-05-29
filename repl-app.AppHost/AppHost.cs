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

var replApi = builder.AddProject<Projects.ReplApi>("replApi")
    .WithReference(publisherDb)
    .WaitFor(publisherDb)
    .WithReference(subscriberDb)
    .WaitFor(subscriberDb);

var familyTreeApi = builder.AddProject<Projects.FamilyTreeApi>("familyTreeApi")
    .WithReference(publisherDb)
    .WaitFor(publisherDb);

builder.Build().Run();
