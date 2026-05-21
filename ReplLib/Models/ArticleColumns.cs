namespace ReplLib.Models;

public class ArticleColumns
{
    public int? column_id { get; set; }
    public string? column { get; set; } 

    public bool? published { get; set; } 

}

// Column name	Data type	Description
// column id	int	Identifier for the column.
// column	sysname	Name of the column.
// published	bit	Whether column is published:

// 0 = No
// 1 = Yes
// publisher type	sysname	Data type of the column at the Publisher.
// subscriber type	sysname	Data type of the column at the Subscriber.