using System.Data;

namespace ReplLib.Models;

public class Article
{
    public int? articleid { get; set; }
    public string? article_name { get; set; } 
    public string? publication { get; set; }
    public static List<Article> FromDataTable(DataTable dt)
    {
        var articles = new List<Article>();
        foreach (DataRow row in dt.Rows)
        {
            var article = new Article
            {
                articleid = row["article id"] != DBNull.Value ? (int?)row["article id"] : null,
                article_name = row["article name"] != DBNull.Value ? row["article name"].ToString() : null,
                //publication = row["publication"] != DBNull.Value ? row["publication"].ToString() : null
            };
            articles.Add(article);
        }
        return articles;
    }
}