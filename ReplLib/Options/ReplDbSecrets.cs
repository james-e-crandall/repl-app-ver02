namespace ReplLib.Options;

public class ReplDbSecrets
{
    public required string PUBLISHERDB_PASSWORD { get; set; } 
    public required string PUBLISHERDB_DATABASENAME { get; set; } 
    public required string SUBSCRIBERDB_PASSWORD { get; set; } 
    public required string SUBSCRIBERDB_DATABASENAME { get; set; } 
}