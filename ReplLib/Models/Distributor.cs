using System.Data;

namespace ReplLib.Models;

    public class Distributor
    {
        public bool Installed { get; set; }
        public string? DistributionServer { get; set; } = string.Empty;
        public bool DistributionDbInstallled { get; set; }
        public bool IsDistributionPublisher { get; set; }
        public bool HasRemoteDistributionPublisher { get; set; }

        public static List<Distributor> FromDataTable(DataTable dt)
        {
            var distributors = new List<Distributor>();
            foreach (DataRow row in dt.Rows)
            {
                var distributor = new Distributor
                {
                    Installed = row["installed"] != DBNull.Value && (bool)row["installed"],
                    DistributionServer = row["distribution server"] != DBNull.Value ? row["distribution server"].ToString() : string.Empty,
                    DistributionDbInstallled = row["distribution db installed"] != DBNull.Value && (bool)row["distribution db installed"],
                    IsDistributionPublisher = row["is distribution publisher"] != DBNull.Value && (bool)row["is distribution publisher"],
                    HasRemoteDistributionPublisher = row["has remote distribution publisher"] != DBNull.Value && (bool)row["has remote distribution publisher"]
                };
                distributors.Add(distributor);
            }
            return distributors;
        }
        

    }