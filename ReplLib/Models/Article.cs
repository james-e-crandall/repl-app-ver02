using System.Data;

namespace ReplLib.Models;

public class Article
{
    public int? articleid { get; set; }
    public string? article_name { get; set; } 
    public string? publication { get; set; }

    public string schema_option { get; set; }

    public static List<Article> FromDataTable(DataTable dt, string publication)
    {
        var articles = new List<Article>();
        foreach (DataRow row in dt.Rows)
        {
            var article = new Article
            {
                articleid = row["article id"] != DBNull.Value ? (int?)row["article id"] : null,
                article_name = row["article name"] != DBNull.Value ? row["article name"].ToString() : null,
                schema_option = Convert.ToHexString((Byte[])row["schema_option"]),
                publication = publication
            };
            articles.Add(article);
        }
        return articles;
    }
}