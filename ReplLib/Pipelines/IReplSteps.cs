using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReplLib.Extensions;
using ReplLib.Models;
using ReplLib.Options;

namespace ReplLib.Pipelines;

[Flags]
public enum ReplState
{
    None = 0,
    HasServerOption = 1,
    HasDistributor = 2,
    HasDistributionDb = 4,
    HasPublisher = 8,
    HasReplicationDbOption = 16,
    HasLogReaderAgent = 32,
    HasPublication = 64,
    HasPublicationSnapshot = 128,
    HasArticle = 256,
    HasSubscription = 512,
    HasPushSubscriptionAgent = 1024
}

public interface IReplSteps
{
    ReplState Order { get; }
    Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext);
}

public sealed partial class ReplicationContext
{
    
}

public class sp_get_distributor(ILogger<sp_get_distributor> logger,
    [FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
{
    public ReplState Order => ReplState.None;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_get_distributor");
        DataTable dt = connection.sp_get_distributor();

        List<Distributor> replicationSetups = dt.AsEnumerable().Select(row =>
            new Distributor
            {
                Installed = row.Field<bool>("installed"),
                DistributionServer = row.Field<string>("distribution server"),
                DistributionDbInstallled = row.Field<bool>("distribution db installed"),
                IsDistributionPublisher = row.Field<bool>("is distribution publisher"),
                HasRemoteDistributionPublisher = row.Field<bool>("has remote distribution publisher"),
            }).ToList();

        foreach(var item in replicationSetups)
        {
            logger.LogInformation($"---> Installed : {item.Installed}");
            if(item.Installed)
            {
                replicationContext.CurrentReplState = replicationContext.CurrentReplState | ReplState.HasServerOption;
                logger.LogInformation($"---> CurrentReplState : {replicationContext.CurrentReplState}");
                replicationContext.CurrentReplState = replicationContext.CurrentReplState | ReplState.HasDistributor;
                logger.LogInformation($"---> CurrentReplState : {replicationContext.CurrentReplState}");
            }
            
            logger.LogInformation($"DistributionServer : {item.DistributionServer}");

            logger.LogInformation($"DistributionDbInstallled : {item.DistributionDbInstallled}");
            if(item.DistributionDbInstallled)
            {
                replicationContext.CurrentReplState = replicationContext.CurrentReplState | ReplState.HasDistributionDb;
            }

            logger.LogInformation($"IsDistributionPublisher : {item.IsDistributionPublisher}");
            if(item.IsDistributionPublisher)
            {
                replicationContext.CurrentReplState = replicationContext.CurrentReplState | ReplState.HasPublisher;
            }

            logger.LogInformation($" HasRemoteDistributionPublisher : {item.HasRemoteDistributionPublisher}");

        }

        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}

public class sp_serveroption(ILogger<sp_get_distributor> logger) : IReplSteps
{
    public ReplState Order => ReplState.HasServerOption;

    public Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_serveroption");
        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
        return Task.CompletedTask;
    }
}