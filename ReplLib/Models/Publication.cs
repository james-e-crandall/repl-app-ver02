using System.Data;

namespace ReplLib.Models;

public class Publication
{
    public int pubid { get; set; }
    public string? name { get; set; }
    public byte status { get; set; }
    public byte replication_frequency { get; set; }
    public byte synchronization_method { get; set; }
    public bool immediate_sync { get; set; }

    public bool allow_push { get; set; }
    public bool allow_pull { get; set; }
    public int has_subscription { get; set; }
    public static List<Publication> FromDataTable(DataTable dt)
    {
        var publications = new List<Publication>();
        foreach (DataColumn column in dt.Columns)
        {
            Console.WriteLine($"Column: {column.ColumnName} | Type: {column.DataType}");
        }
        foreach (DataRow row in dt.Rows)
        {
            var publication = new Publication
            {
                pubid = row["pubid"] != DBNull.Value ? (int)row["pubid"] : 0,
                name = row["name"] != DBNull.Value ? row["name"].ToString() : null,
                status = row["status"] != DBNull.Value ? (byte)row["status"] : (byte)0,
                replication_frequency = row["replication frequency"] != DBNull.Value ? (byte)row["replication frequency"] : (byte)0,
                synchronization_method = row["synchronization method"] != DBNull.Value ? (byte)row["synchronization method"] : (byte)0,
                immediate_sync = row["immediate_sync"] != DBNull.Value ? (bool)row["immediate_sync"] : false,
                allow_push = row["allow_push"] != DBNull.Value ? (bool)row["allow_push"] : false,
                allow_pull = row["allow_pull"] != DBNull.Value ? (bool)row["allow_pull"] : false,
                has_subscription = row["has subscription"] != DBNull.Value ? (int)row["has subscription"] : 0
            };
            publications.Add(publication);
        }
        return publications;
    }

}