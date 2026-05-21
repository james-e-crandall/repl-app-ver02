using System.Diagnostics;
using Microsoft.Data.SqlClient;
using ReplLib.Pipelines;

namespace ReplService;

public class Worker(IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{

    public const string ActivitySourceName = "Replication";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = s_activitySource.StartActivity(
            "Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();

            var replicationPipeline = scope.ServiceProvider.GetRequiredService<ReplicationPipeline>();
            await replicationPipeline.Run(stoppingToken);


        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }
}
