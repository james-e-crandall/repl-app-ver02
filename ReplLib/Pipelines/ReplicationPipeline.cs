using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace ReplLib.Pipelines;

public sealed class ReplicationPipeline(ILogger<ReplicationPipeline> logger, IEnumerable<IReplSteps> steps)
{

    public async Task Run(CancellationToken ct)
    {
        var replContext = new ReplicationContext();

        foreach(var step in steps.OrderBy(x => x.Order))
        {
            Console.WriteLine($"Executing Order --> step {step.Order}");
        }

        foreach(var step in steps.OrderBy(x => x.Order))
        {
            logger.LogInformation($"Executing --> step {step.Order}");
            await step.EvaulateAsync(ct, replContext);
        }
    }

}

