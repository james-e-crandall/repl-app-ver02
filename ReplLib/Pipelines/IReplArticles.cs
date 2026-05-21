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