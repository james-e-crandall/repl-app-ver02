using System.Data;

namespace ReplLib.Models;

public class ReplAgent
{
    public int? id { get; set; }
    public string? name { get; set; } = string.Empty;
    public int publisher_security_mode { get; set; }
    public string? publisher_login { get; set; }
    public string? publisher_password { get; set; }
    public Guid? job_id { get; set; }
    public string? job_login { get; set; }
    public string? job_password { get; set; }

    public static List<ReplAgent> FromDataTable(DataTable dt)
    {
        var replAgents = new List<ReplAgent>();
        foreach (DataColumn column in dt.Columns)
        {
            Console.WriteLine($"Column: {column.ColumnName} | Type: {column.DataType}");
        }
        foreach (DataRow row in dt.Rows)
        {
            var replAgent = new ReplAgent
            {
                id = row["id"] != DBNull.Value ? (int?)row["id"] : null,
                name = row["name"] != DBNull.Value ? row["name"].ToString() : null,
                publisher_security_mode = Convert.ToInt32(row["publisher_security_mode"]),
                publisher_login = row["publisher_login"] != DBNull.Value ? row["publisher_login"].ToString() : null,
                publisher_password = row["publisher_password"] != DBNull.Value ? row["publisher_password"].ToString() : null,
                job_id = row["job_id"] != DBNull.Value ? (Guid?)row["job_id"] : null,
                job_login = row["job_login"] != DBNull.Value ? row["job_login"].ToString() : null,
                job_password = row["job_password"] != DBNull.Value ? row["job_password"].ToString() : null
            };
            replAgents.Add(replAgent);
        }
        return replAgents;
    }
}


//1	2014C4121C30-publisherDb-1	0	sa	**********	5ABAF2B0-97D3-4F7F-A807-635DE33BE3AD	NULL	**********