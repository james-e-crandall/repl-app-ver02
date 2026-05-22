namespace ReplLib.Pipelines;

public interface IReplArticles
{
    public IEnumerable<ReplArticle> GetTableNames();
}

public class ReplArticle
{
    public required string ArticleName { get; set; }
    public required string PublicationName { get; set; }
}

public static class ReplArticleExtensions
{
    public static IEnumerable<string> PublicationsNames(this IEnumerable<ReplArticle> replArticles)
    {
        foreach (var publication in replArticles.Select(a => a.PublicationName).Distinct())
        {
            yield return publication;
        }
    }
}