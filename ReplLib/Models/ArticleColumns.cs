using System.Data;

namespace ReplLib.Models;

public class ArticleColumns
{
    public int? column_id { get; set; }
    public string? column { get; set; } 

    public bool? published { get; set; } 
    public string? article { get; set; }
    public string? publication { get; set; }

    public static List<ArticleColumns> FromDataTable(DataTable dt, string article, string publication)
    {
        var articleColumns = new List<ArticleColumns>();
        foreach (DataRow row in dt.Rows)
        {
            var articleColumn = new ArticleColumns
            {
                column_id = row["column id"] != DBNull.Value ? (int?)row["column id"] : null,
                column = row["column"] != DBNull.Value ? row["column"].ToString() : null,
                published = row["published"] != DBNull.Value && (bool)row["published"],
                article = article,
                publication = publication
            };
            articleColumns.Add(articleColumn);
        }
        return articleColumns;
    }

}

// Column name	Data type	Description
// column id	int	Identifier for the column.
// column	sysname	Name of the column.
// published	bit	Whether column is published:

// 0 = No
// 1 = Yes
// publisher type	sysname	Data type of the column at the Publisher.
// subscriber type	sysname	Data type of the column at the Subscriber.