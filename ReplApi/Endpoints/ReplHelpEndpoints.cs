using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using ReplLib.Extensions;
using ReplLib.Models;
using ReplLib.Pipelines;

namespace ReplApi.Endpoints;

public static class ReplHelpEndpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        var replItems = app.MapGroup("/repl-help").WithTags("Repl");

        replItems.MapGet("/GetDistributor", sp_get_distributor);
        replItems.MapGet("/GetLogReaderAgent", sp_helplogreader_agent);
        replItems.MapGet("/GetPublication", sp_helppublication);
        replItems.MapGet("/GetPublicationSnapshot", sp_helppublication_snapshot);
        replItems.MapGet("/GetArticle", sp_helparticle);
        replItems.MapGet("/GetArticleColumns", sp_helparticlecolumns);

        static async Task<Results<Ok<List<Distributor>>, NotFound>> sp_get_distributor(ILogger<Program> logger, [FromKeyedServices("publisherDb")] SqlConnection connection)
        {
            var dt = connection.sp_get_distributor();
            var replicationSetups = Distributor.FromDataTable(dt);
            return TypedResults.Ok<List<Distributor>>(replicationSetups);

        }

        static async Task<Results<Ok<List<ReplAgent>>, NotFound>> sp_helplogreader_agent(ILogger<Program> logger, [FromKeyedServices("publisherDb")] SqlConnection connection)
        {
            var dt = connection.sp_helplogreader_agent();
            var replicationSetups = ReplAgent.FromDataTable(dt);
            return TypedResults.Ok<List<ReplAgent>>(replicationSetups);
        }

        static async Task<Results<Ok<List<Publication>>, NotFound>> sp_helppublication(ILogger<Program> logger, [FromKeyedServices("publisherDb")] SqlConnection connection,
            IEnumerable<ReplArticle> replArticles)
        {
            var replicationSetups = new List<Publication>();
            // var publications = replArticles.PublicationsNames();
            // foreach(var publication in publications)
            // {
            //     var dt = connection.sp_helppublication(publication);
            //     replicationSetups.AddRange(Publication.FromDataTable(dt));
            // }
            var dt = connection.sp_helppublication();
            replicationSetups.AddRange(Publication.FromDataTable(dt));
            return TypedResults.Ok<List<Publication>>(replicationSetups);
        }

        static async Task<Results<Ok<List<ReplAgent>>, NotFound>> sp_helppublication_snapshot(ILogger<Program> logger, [FromKeyedServices("publisherDb")] SqlConnection connection,
            IEnumerable<ReplArticle> replArticles)
        {
            var replicationSetups = new List<ReplAgent>();
            var publications = new List<Publication>();
            var dt1 = connection.sp_helppublication();
            publications.AddRange(Publication.FromDataTable(dt1));
            foreach(var publication in publications)            {
                var dt = connection.sp_helppublication_snapshot(publication.name);
                replicationSetups.AddRange(ReplAgent.FromDataTable(dt));
            }
            return TypedResults.Ok<List<ReplAgent>>(replicationSetups);
        }

        static async Task<Results<Ok<List<Article>>, NotFound>> sp_helparticle(ILogger<Program> logger, [FromKeyedServices("publisherDb")] SqlConnection connection,
            IEnumerable<ReplArticle> replArticles)
        {
            var replicationSetups = new List<Article>();
            var publications = new List<Publication>();
            var dt1 = connection.sp_helppublication();
            publications.AddRange(Publication.FromDataTable(dt1));
            foreach(var publication in publications)
            {
                var dt = connection.sp_helparticle(publication.name);
                replicationSetups.AddRange(Article.FromDataTable(dt, publication.name));
            }

            return TypedResults.Ok<List<Article>>(replicationSetups);
        }

        static async Task<Results<Ok<List<ArticleColumns>>, NotFound>> sp_helparticlecolumns(ILogger<Program> logger, [FromKeyedServices("publisherDb")] SqlConnection connection,
            IEnumerable<ReplArticle> replArticles)
        {
            var replicationSetups = new List<ArticleColumns>();

            var publications = new List<Publication>();
            var dt1 = connection.sp_helppublication();
            publications.AddRange(Publication.FromDataTable(dt1));

            var articles = new List<Article>();
            foreach(var publication in publications)
            {
                var dt = connection.sp_helparticle(publication.name);
                articles.AddRange(Article.FromDataTable(dt, publication.name));
            }

            foreach(var article in articles)
            {
                var dt = connection.sp_helparticlecolumns(article.publication, article.article_name);
                replicationSetups.AddRange(ArticleColumns.FromDataTable(dt, article.publication, article.article_name));
            }

            return TypedResults.Ok<List<ArticleColumns>>(replicationSetups);
        }

    }
}