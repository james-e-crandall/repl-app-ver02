using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlServer");

var publisherDb = sqlServer.AddDatabase("publisherDb");
var subscriberDb = sqlServer.AddDatabase("subscriberDb");

var replService = builder.AddProject<Projects.ReplService>("replService")
    .WithReference(publisherDb)
    .WaitFor(publisherDb)
    .WithReference(subscriberDb)
    .WaitFor(subscriberDb); 


builder.Build().Run();
