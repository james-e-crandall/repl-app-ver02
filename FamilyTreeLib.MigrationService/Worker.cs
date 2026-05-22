using System.Diagnostics;
using FamilyTreeLib.Data;
using FamilyTreeLib.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyTreeLib.MigrationService;

public class Worker(IServiceProvider serviceProvider,
    IHostEnvironment hostEnvironment,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    private readonly ActivitySource _activitySource = new(hostEnvironment.ApplicationName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity(hostEnvironment.ApplicationName, ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FamilyTreeContext>();

            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedDataAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(FamilyTreeContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedDataAsync(
        FamilyTreeContext dbContext, CancellationToken cancellationToken)
    {

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database
                .BeginTransactionAsync(cancellationToken);

            // await dbContext.Halflings.AddRangeAsync(
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/"), Name = "Balbo", YearOfBirth = 1167 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/1/"), Name = "Mungo", YearOfBirth = 1207 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/2/"), Name = "Pansy", YearOfBirth = 1212 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/3/"), Name = "Ponto", YearOfBirth = 1216 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/4/"), Name = "Largo", YearOfBirth = 1220 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/5/"), Name = "Lily", YearOfBirth = 1222 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/1/1/"), Name = "Bungo", YearOfBirth = 1246 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/1/2/"), Name = "Belba", YearOfBirth = 1256 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/1/3/"), Name = "Longo", YearOfBirth = 1260 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/1/4/"), Name = "Linda", YearOfBirth = 1262 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/1/5/"), Name = "Bingo", YearOfBirth = 1264 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/3/1/"), Name = "Rosa", YearOfBirth = 1256 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/3/2/"), Name = "Polo" },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/4/1/"), Name = "Fosco", YearOfBirth = 1264 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/1/1/1/"), Name = "Bilbo", YearOfBirth = 1290 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/1/3/1/"), Name = "Otho", YearOfBirth = 1310 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/1/5/1/"), Name = "Falco", YearOfBirth = 1303 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/3/2/1/"), Name = "Posco", YearOfBirth = 1302 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/3/2/2/"), Name = "Prisca", YearOfBirth = 1306 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/4/1/1/"), Name = "Dora", YearOfBirth = 1302 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/4/1/2/"), Name = "Drogo", YearOfBirth = 1308 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/4/1/3/"), Name = "Dudo", YearOfBirth = 1311 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/1/3/1/1/"), Name = "Lotho", YearOfBirth = 1310 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/1/5/1/1/"), Name = "Poppy", YearOfBirth = 1344 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/3/2/1/1/"), Name = "Ponto", YearOfBirth = 1346 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/3/2/1/2/"), Name = "Porto", YearOfBirth = 1348 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/3/2/1/3/"), Name = "Peony", YearOfBirth = 1350 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/4/1/2/1/"), Name = "Frodo", YearOfBirth = 1368 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/4/1/3/1/"), Name = "Daisy", YearOfBirth = 1350 },
            //     new Halfling { PathFromPatriarch = HierarchyId.Parse("/3/2/1/1/1/"), Name = "Angelica", YearOfBirth = 1381 });

            await dbContext.Halflings.AddRangeAsync(
                new Halfling(HierarchyId.Parse("/"), "Balbo", 1167),
                new Halfling(HierarchyId.Parse("/1/"), "Mungo", 1207),
                new Halfling(HierarchyId.Parse("/2/"), "Pansy", 1212),
                new Halfling(HierarchyId.Parse("/3/"), "Ponto", 1216),
                new Halfling(HierarchyId.Parse("/4/"), "Largo", 1220),
                new Halfling(HierarchyId.Parse("/5/"), "Lily", 1222),
                new Halfling(HierarchyId.Parse("/1/1/"), "Bungo", 1246),
                new Halfling(HierarchyId.Parse("/1/2/"), "Belba", 1256),
                new Halfling(HierarchyId.Parse("/1/3/"), "Longo", 1260),
                new Halfling(HierarchyId.Parse("/1/4/"), "Linda", 1262),
                new Halfling(HierarchyId.Parse("/1/5/"), "Bingo", 1264),
                new Halfling(HierarchyId.Parse("/3/1/"), "Rosa", 1256),
                new Halfling(HierarchyId.Parse("/3/2/"), "Polo"),
                new Halfling(HierarchyId.Parse("/4/1/"), "Fosco", 1264),
                new Halfling(HierarchyId.Parse("/1/1/1/"), "Bilbo", 1290),
                new Halfling(HierarchyId.Parse("/1/3/1/"), "Otho", 1310),
                new Halfling(HierarchyId.Parse("/1/5/1/"), "Falco", 1303),
                new Halfling(HierarchyId.Parse("/3/2/1/"), "Posco", 1302),
                new Halfling(HierarchyId.Parse("/3/2/2/"), "Prisca", 1306),
                new Halfling(HierarchyId.Parse("/4/1/1/"), "Dora", 1302),
                new Halfling(HierarchyId.Parse("/4/1/2/"), "Drogo", 1308),
                new Halfling(HierarchyId.Parse("/4/1/3/"), "Dudo", 1311),
                new Halfling(HierarchyId.Parse("/1/3/1/1/"), "Lotho", 1310),
                new Halfling(HierarchyId.Parse("/1/5/1/1/"), "Poppy", 1344),
                new Halfling(HierarchyId.Parse("/3/2/1/1/"), "Ponto", 1346),
                new Halfling(HierarchyId.Parse("/3/2/1/2/"), "Porto", 1348),
                new Halfling(HierarchyId.Parse("/3/2/1/3/"), "Peony", 1350),
                new Halfling(HierarchyId.Parse("/4/1/2/1/"), "Frodo", 1368),
                new Halfling(HierarchyId.Parse("/4/1/3/1/"), "Daisy", 1350),
                new Halfling(HierarchyId.Parse("/3/2/1/1/1/"), "Angelica", 1381));

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }


}
