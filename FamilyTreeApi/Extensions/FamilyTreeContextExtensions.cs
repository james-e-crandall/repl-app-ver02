using FamilyTreeLib.Data;
using FamilyTreeLib.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyTreeApi.Extensions;

public static class FamilyTreeContextExtensions
{
    public static async Task<Halfling?> FindDirectAncestor(this FamilyTreeContext context, string name)
        => await context.Halflings
            .SingleOrDefaultAsync(
                ancestor => ancestor.PathFromPatriarch == context.Halflings
                    .Single(descendent => descendent.Name == name).PathFromPatriarch
                    .GetAncestor(1));

    public static IQueryable<Halfling> FindDirectDescendents(this FamilyTreeContext context, string name)
        => context.Halflings.Where(
            descendent => descendent.PathFromPatriarch.GetAncestor(1) == context.Halflings
                .Single(ancestor => ancestor.Name == name).PathFromPatriarch);


    public static IQueryable<Halfling> FindAllAncestors(this FamilyTreeContext context, string name)
        => context.Halflings.Where(
                ancestor => context.Halflings
                    .Single(
                        descendent =>
                            descendent.Name == name
                            && ancestor.Id != descendent.Id)
                    .PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch))
            .OrderByDescending(ancestor => ancestor.PathFromPatriarch.GetLevel());

    public static IQueryable<Halfling> FindAllDescendents(this FamilyTreeContext context, string name)
        => context.Halflings.Where(
                descendent => descendent.PathFromPatriarch.IsDescendantOf(
                    context.Halflings
                        .Single(
                            ancestor =>
                                ancestor.Name == name
                                && descendent.Id != ancestor.Id)
                        .PathFromPatriarch))
            .OrderBy(descendent => descendent.PathFromPatriarch.GetLevel());

    public static async Task<Halfling?> FindCommonAncestor(this FamilyTreeContext context, string first, string second)
        => await context.Halflings
            .Where(
                ancestor => context.Halflings.Single(h => h.Name == first).PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch)
                            && context.Halflings.Single(h => h.Name == second).PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch))
            .OrderByDescending(ancestor => ancestor.PathFromPatriarch.GetLevel())
            .FirstOrDefaultAsync();

    public static async Task<Halfling?> FindCommonAncestor(this FamilyTreeContext context, Halfling first, Halfling second)
        => await context.Halflings
            .Where(
                ancestor => first.PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch)
                            && second.PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch))
            .OrderByDescending(ancestor => ancestor.PathFromPatriarch.GetLevel())
            .FirstOrDefaultAsync();

    public static async Task SetReparentedValue(this FamilyTreeContext context, string ancestorName, string newParentName)
    {
        var mungo = await context.Halflings.SingleAsync(halfling => halfling.Name == ancestorName);
        var ponto = await context.Halflings.SingleAsync(halfling => halfling.Name == newParentName);

        var longoAndDescendents = await context.Halflings.Where(
                descendent => descendent.PathFromPatriarch.IsDescendantOf(
                    context.Halflings.Single(ancestor => ancestor.Name == ancestorName).PathFromPatriarch))
            .ToListAsync();

        foreach (var descendent in longoAndDescendents)
        {
            descendent.PathFromPatriarch
                = descendent.PathFromPatriarch.GetReparentedValue(
                    mungo.PathFromPatriarch, ponto.PathFromPatriarch)!;
        }

        await context.SaveChangesAsync();

    }



}