namespace Jobs.BackgroundJobs.HangfireScheduler.Models;

public class ApplicationSettings
{
    public UrlModel Urls { get; set; }

    public class UrlModel
    {
        public string AnimalMarketApi { get; set; }
        public AnimalMarketModule AnimalMarket { get; set; }

        public string HirovoApi { get; set; }
        public HirovoModule Hirovo { get; set; }

        public class AnimalMarketModule
        {
            public string CheckExpiredBids { get; set; }
            public string SendVeterinaryNotifications { get; set; }
            public string CheckExpiredPayments { get; set; }
            public string NotifyEndingAuctions { get; set; }
            public string NotifyApproachingDisputeDeadlines { get; set; }
            public string AutoReleaseEscrowAfterDisputePeriod { get; set; }
        }

        public class HirovoModule
        {
            public string SeedAndTranslateSkills { get; set; }
        }
    }
}