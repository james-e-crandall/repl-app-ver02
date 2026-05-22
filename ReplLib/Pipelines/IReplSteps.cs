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

public class sp_serveroption(ILogger<sp_get_distributor> logger,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
{
    public ReplState Order => ReplState.HasServerOption;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_serveroption");
        await connection.sp_serveroption(ct);
        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}

public class sp_adddistributor(ILogger<sp_adddistributor> logger,IOptions<ReplDbSecrets> dbsecretsOptions,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
{
    public ReplState Order => ReplState.HasDistributor;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_adddistributor");
        await connection.sp_adddistributor(ct, dbsecretsOptions.Value.PUBLISHERDB_PASSWORD);
        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}

public class sp_adddistributiondb(ILogger<sp_adddistributiondb> logger, IOptions<ReplDbSecrets> dbsecretsOptions,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
{
    public ReplState Order => ReplState.HasDistributionDb;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_adddistributiondb");
        await  connection.sp_adddistributiondb(ct, "sa", dbsecretsOptions.Value.PUBLISHERDB_PASSWORD);
        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}

public class sp_adddistpublisher(ILogger<sp_adddistpublisher> logger, IOptions<ReplDbSecrets> dbsecretsOptions,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
{
    public ReplState Order => ReplState.HasPublisher;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_adddistpublisher");
        await  connection.sp_adddistpublisher(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, "sa", dbsecretsOptions.Value.PUBLISHERDB_PASSWORD);
        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}

public class sp_replicationdboption(ILogger<sp_replicationdboption> logger, IOptions<ReplDbSecrets> dbsecretsOptions,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
{
    public ReplState Order => ReplState.HasReplicationDbOption;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_replicationdboption");
        await  connection.sp_replicationdboption(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME);
        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}

public class sp_addlogreader_agent(ILogger<sp_addlogreader_agent> logger,IOptions<ReplDbSecrets> dbsecretsOptions,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
{
    public ReplState Order => ReplState.HasLogReaderAgent;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_addlogreader_agent");
        await  connection.sp_addlogreader_agent(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, "sa", dbsecretsOptions.Value.PUBLISHERDB_PASSWORD);
        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}
// public class sp_addpublication(ILogger<sp_addpublication> logger, IOptions<ReplDbSecrets> dbsecretsOptions, IEnumerable<IReplArticles> replArticles,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
// {
//     public ReplState Order => ReplState.HasPublication;

//     public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
//     {
//         logger.LogInformation("Evaluating sp_addlogreader_agent");
//         var publications = replArticles.SelectMany(x => x.GetTableNames().Select(a => a.PublicationName)).Distinct();
//         foreach(var publication in publications)
//         {
//             await connection.sp_addpublication(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, publication);
//         }

//         replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
//     }
// }

public class sp_addpublication(ILogger<sp_addpublication> logger, IOptions<ReplDbSecrets> dbsecretsOptions, IEnumerable<ReplArticle> replArticles,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
{
    public ReplState Order => ReplState.HasPublication;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_addlogreader_agent");
        var publications = replArticles.Select(a => a.PublicationName).Distinct();
        foreach(var publication in publications)
        {
            await connection.sp_addpublication(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, publication);
        }

        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}

// public class sp_addpublication_snapshot(ILogger<sp_addpublication_snapshot> logger, IOptions<ReplDbSecrets> dbsecretsOptions, IEnumerable<IReplArticles> replArticles,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
// {
//     public ReplState Order => ReplState.HasPublicationSnapshot;

//     public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
//     {
//         logger.LogInformation("Evaluating sp_addpublication_snapshot");
//         var publications = replArticles.SelectMany(x => x.GetTableNames().Select(a => a.PublicationName)).Distinct();
//         foreach(var publication in publications)
//         {
//             await connection.sp_addpublication_snapshot(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, publication, "sa", dbsecretsOptions.Value.PUBLISHERDB_PASSWORD);
//         }
//         replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
//     }
// }

public class sp_addpublication_snapshot(ILogger<sp_addpublication_snapshot> logger, IOptions<ReplDbSecrets> dbsecretsOptions, IEnumerable<ReplArticle> replArticles,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
{
    public ReplState Order => ReplState.HasPublicationSnapshot;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_addpublication_snapshot");
        var publications = replArticles.Select(a => a.PublicationName).Distinct();
        foreach(var publication in publications)
        {
            await connection.sp_addpublication_snapshot(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, publication, "sa", dbsecretsOptions.Value.PUBLISHERDB_PASSWORD);
        }
        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}

// public class sp_addarticle (ILogger<sp_addarticle> logger,IOptions<ReplDbSecrets> dbsecretsOptions, IEnumerable<IReplArticles> replArticles,[FromKeyedServices("publisherDb")] SqlConnection connection)  : IReplSteps
// {
//     public ReplState Order => ReplState.HasArticle;

//     public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
//     {
//         logger.LogInformation("Evaluating sp_addarticle");
//         foreach(var article in replArticles)
//         {
//             var tableNames = article.GetTableNames();
//             foreach(var tableName in tableNames)
//             {
//                 await connection.sp_addarticle(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, tableName.PublicationName, tableName.ArticleName);
//             }
//         }
//         replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
//     }
// }

public class sp_addarticle (ILogger<sp_addarticle> logger,IOptions<ReplDbSecrets> dbsecretsOptions, IEnumerable<ReplArticle> replArticles,[FromKeyedServices("publisherDb")] SqlConnection connection)  : IReplSteps
{
    public ReplState Order => ReplState.HasArticle;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_addarticle");
        foreach(var tableName in replArticles)
        {
            await connection.sp_addarticle(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, tableName.PublicationName, tableName.ArticleName);
        }

        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}

// public class sp_addsubscription(ILogger<sp_addsubscription> logger, IOptions<ReplDbSecrets> dbsecretsOptions, IEnumerable<IReplArticles> replArticles,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
// {
//     public ReplState Order => ReplState.HasSubscription;

//     public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
//     {
//         logger.LogInformation("Evaluating sp_addsubscription");
//         var publications = replArticles.SelectMany(x => x.GetTableNames().Select(a => a.PublicationName)).Distinct();
//         foreach(var publication in publications)
//         {
//             await connection.sp_addsubscription(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, publication, dbsecretsOptions.Value.SUBSCRIBERDB_DATABASENAME);
//         }
//         replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
//     }
// }

public class sp_addsubscription(ILogger<sp_addsubscription> logger, IOptions<ReplDbSecrets> dbsecretsOptions, IEnumerable<ReplArticle> replArticles,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
{
    public ReplState Order => ReplState.HasSubscription;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_addsubscription");
        var publications = replArticles.Select(a => a.PublicationName).Distinct();
        foreach(var publication in publications)
        {
            await connection.sp_addsubscription(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, publication, dbsecretsOptions.Value.SUBSCRIBERDB_DATABASENAME);
        }
        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}

// public class sp_addpushsubscription_agent(ILogger<sp_addpushsubscription_agent> logger, IOptions<ReplDbSecrets> dbsecretsOptions, IEnumerable<IReplArticles> replArticles,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
// {
//     public ReplState Order => ReplState.HasPushSubscriptionAgent;

//     public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
//     {
//         logger.LogInformation("Evaluating sp_addpushsubscription_agent");
//         var publications = replArticles.SelectMany(x => x.GetTableNames().Select(a => a.PublicationName)).Distinct();
//         foreach(var publication in publications)
//         {
//            await connection.sp_addpushsubscription_agent(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, publication, dbsecretsOptions.Value.SUBSCRIBERDB_DATABASENAME, "sa", dbsecretsOptions.Value.PUBLISHERDB_PASSWORD);
//         }
//         replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
//     }
// }

public class sp_addpushsubscription_agent(ILogger<sp_addpushsubscription_agent> logger, IOptions<ReplDbSecrets> dbsecretsOptions, IEnumerable<ReplArticle> replArticles,[FromKeyedServices("publisherDb")] SqlConnection connection) : IReplSteps
{
    public ReplState Order => ReplState.HasPushSubscriptionAgent;

    public async Task EvaulateAsync(CancellationToken ct, ReplicationContext replicationContext)
    {
        logger.LogInformation("Evaluating sp_addpushsubscription_agent");
        var publications = replArticles.Select(a => a.PublicationName).Distinct();
        foreach(var publication in publications)
        {
           await connection.sp_addpushsubscription_agent(ct, dbsecretsOptions.Value.PUBLISHERDB_DATABASENAME, publication, dbsecretsOptions.Value.SUBSCRIBERDB_DATABASENAME, "sa", dbsecretsOptions.Value.PUBLISHERDB_PASSWORD);
        }
        replicationContext.CurrentReplState = replicationContext.CurrentReplState | this.Order;
    }
}

