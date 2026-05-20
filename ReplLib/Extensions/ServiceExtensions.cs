

using Microsoft.Extensions.DependencyInjection;
using ReplLib.Pipelines;

public static class ServiceExtensions
{
    public static IServiceCollection AddReplicationPipeline(this IServiceCollection services)
    {
        services.AddScoped<ReplicationPipeline>();
        var interfaceType = typeof(IReplSteps);
        var implementations = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => interfaceType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

        foreach (var implementation in implementations)
        {
            services.AddScoped(interfaceType, implementation);
        }

        return services;
    }

}

