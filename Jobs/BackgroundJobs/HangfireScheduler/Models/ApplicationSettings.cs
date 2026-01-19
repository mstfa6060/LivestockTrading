namespace Jobs.BackgroundJobs.HangfireScheduler.Models;

public class ApplicationSettings
{
    public UrlModel Urls { get; set; }

    public class UrlModel
    {
        /// <summary>
        /// LivestockTrading API base URL
        /// </summary>
        public string LivestockTradingApi { get; set; }
    }
}
