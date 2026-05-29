using FamilyTreeApi.Extensions;
using FamilyTreeLib.Data;
using FamilyTreeLib.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FamilyTreeApi.Endpoints;

public static class HalflingsEndpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        var halflingItems = app.MapGroup("/halflings").WithTags("Halflings");

        halflingItems.MapGet("/", GetAll);
        halflingItems.MapGet("/{name}", GetByName);

        halflingItems.MapGet("/FindDirectAncestor/{name}", FindDirectAncestor);
        halflingItems.MapGet("/FindDirectDescendents/{name}", FindDirectDescendents);

        halflingItems.MapGet("/FindAllAncestors/{name}", FindAllAncestors);
        halflingItems.MapGet("/FindAllDescendents/{name}", FindAllDescendents);

        halflingItems.MapGet("/FindCommonAncestor/{first}/{second}", FindCommonAncestor);
        
        halflingItems.MapGet("/SetReparentedValue/{ancestorName}/{newParentName}", SetReparentedValue);

    }

    private static Results<Ok<List<Halfling>>, NotFound>  GetAll(ILogger<Program> logger, [FromKeyedServices] FamilyTreeContext context)
    {
        var halflings = context.Halflings.ToList();
        return TypedResults.Ok(halflings);
    }

    public static Results<Ok<Halfling>, NotFound> GetByName(ILogger<Program> logger, [FromKeyedServices] FamilyTreeContext context, string name)
    {
        var halfling = context.Halflings.SingleOrDefault(h => h.Name == name);
        if (halfling is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(halfling);
    }

    private static async Task<Results<Ok<Halfling>, NotFound>> FindDirectAncestor(ILogger<Program> logger, [FromKeyedServices] FamilyTreeContext context, string name)
    {
        var ancestor = await context.FindDirectAncestor(name);
        if (ancestor is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(ancestor);
    }

    private static Results<Ok<List<Halfling>>, NotFound> FindDirectDescendents([FromKeyedServices] FamilyTreeContext context, string name)
    {
        var descendents = context.FindDirectDescendents(name).ToList();
        return TypedResults.Ok(descendents);
    }  

    private static Results<Ok<List<Halfling>>, NotFound> FindAllAncestors([FromKeyedServices] FamilyTreeContext context, string name)
    {
        var ancestors = context.FindAllAncestors(name).ToList();
        return TypedResults.Ok(ancestors);
    }

    private static Results<Ok<List<Halfling>>, NotFound> FindAllDescendents([FromKeyedServices] FamilyTreeContext context, string name)
    {
        var descendents = context.FindAllDescendents(name).ToList();
        return TypedResults.Ok(descendents);
    }

    private static async Task<Results<Ok<Halfling>, NotFound>> FindCommonAncestor(ILogger<Program> logger, [FromKeyedServices] FamilyTreeContext context, string first, string second)
    {
        var ancestor = await context.FindCommonAncestor(first, second);
        if (ancestor is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(ancestor);
    }

    private static async Task<Results<Ok<bool>, NotFound>> SetReparentedValue(ILogger<Program> logger, [FromKeyedServices] FamilyTreeContext context, string ancestorName, string newParentName)
    {
        await context.SetReparentedValue(ancestorName, newParentName);
        return TypedResults.Ok(true);
    }
}