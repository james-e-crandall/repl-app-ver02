namespace ReplLib.Models;

    public class Distributor
    {
        public bool Installed { get; set; }
        public string? DistributionServer { get; set; } = string.Empty;
        public bool DistributionDbInstallled { get; set; }
        public bool IsDistributionPublisher { get; set; }
        public bool HasRemoteDistributionPublisher { get; set; }
    }