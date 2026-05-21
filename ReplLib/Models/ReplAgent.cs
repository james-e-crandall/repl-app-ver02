namespace ReplLib.Models;

public class ReplAgent
{
    public int? id { get; set; }
    public string? name { get; set; } = string.Empty;
    public int? publisher_security_mode { get; set; }

    public string? publisher_login { get; set; }
    public string? publisher_password { get; set; }
    public Guid? job_id { get; set; }
    public string? job_login { get; set; }
    public string? job_password { get; set; }
}


//1	2014C4121C30-publisherDb-1	0	sa	**********	5ABAF2B0-97D3-4F7F-A807-635DE33BE3AD	NULL	**********