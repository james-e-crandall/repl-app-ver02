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

}