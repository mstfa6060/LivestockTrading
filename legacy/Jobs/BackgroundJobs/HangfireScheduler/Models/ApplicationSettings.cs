namespace Jobs.BackgroundJobs.HangfireScheduler.Models;

public class ApplicationSettings
{
    public UrlModel Urls { get; set; }

    /// <summary>
    /// MaxMind GeoLite2 .mmdb dosya yolu (Hangfire job bu path'e yazar)
    /// </summary>
    public string GeoIpDatabasePath { get; set; }

    public class UrlModel
    {
        /// <summary>
        /// LivestockTrading API base URL
        /// </summary>
        public string LivestockTradingApi { get; set; }

        /// <summary>
        /// IAM API base URL (GeoIP reload notify için)
        /// </summary>
        public string IamApi { get; set; }
    }
}
