namespace Jobs.BackgroundJobs.HangfireScheduler.Models;

public class ApplicationSettings
{
    public UrlModel Urls { get; set; }

    public class UrlModel
    {
        public string LivestockTradingApi { get; set; }

        // TODO: Add LivestockTrading module endpoints here
        // public LivestockTradingModule LivestockTrading { get; set; }
    }
}
