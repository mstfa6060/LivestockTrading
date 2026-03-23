using Microsoft.EntityFrameworkCore;
using Common.Definitions.Infrastructure.RelationalDB;
using Common.Services.Environment;
using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Infrastructure.RelationalDB;

public class LivestockTradingModuleDbContext : DefinitionDbContext, ILivestockTradingModuleDbContext
{
    public LivestockTradingModuleDbContext(LivestockTradingDbContextOptions customDbContextOptions)
        : base(customDbContextOptions.DefinitionDbContextOptions)
    { }

    /// <summary>
    /// Design-time constructor for EF Core migrations
    /// </summary>
    public LivestockTradingModuleDbContext(DbContextOptions<DefinitionDbContext> options)
        : base(options)
    { }

    // Entity Sets - Categories & Products
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductPrice> ProductPrices { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    // Location & Sellers
    public DbSet<Location> Locations { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Farm> Farms { get; set; }

    // Reviews & User Related
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<SellerReview> SellerReviews { get; set; }
    public DbSet<FavoriteProduct> FavoriteProducts { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<SearchHistory> SearchHistories { get; set; }
    public DbSet<ProductViewHistory> ProductViewHistories { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }

    // Product Specific Info
    public DbSet<AnimalInfo> AnimalInfos { get; set; }
    public DbSet<HealthRecord> HealthRecords { get; set; }
    public DbSet<Vaccination> Vaccinations { get; set; }
    public DbSet<ChemicalInfo> ChemicalInfos { get; set; }
    public DbSet<MachineryInfo> MachineryInfos { get; set; }
    public DbSet<SeedInfo> SeedInfos { get; set; }
    public DbSet<FeedInfo> FeedInfos { get; set; }
    public DbSet<VeterinaryInfo> VeterinaryInfos { get; set; }

    // Messaging
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Offer> Offers { get; set; }

    // Helpers & Configuration
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<ShippingCarrier> ShippingCarriers { get; set; }
    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<TaxRate> TaxRates { get; set; }
    public DbSet<ShippingZone> ShippingZones { get; set; }
    public DbSet<ShippingRate> ShippingRates { get; set; }

    // Contact
    public DbSet<ContactForm> ContactForms { get; set; }

    // Reports
    public DbSet<ProductReport> ProductReports { get; set; }

    // Subscriptions & Monetization
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<SellerSubscription> SellerSubscriptions { get; set; }
    public DbSet<IAPTransaction> IAPTransactions { get; set; }
    public DbSet<BoostPackage> BoostPackages { get; set; }
    public DbSet<ProductBoost> ProductBoosts { get; set; }

    // Deals & Transport
    public DbSet<Deal> Deals { get; set; }
    public DbSet<Transporter> Transporters { get; set; }
    public DbSet<TransporterReview> TransporterReviews { get; set; }
    public DbSet<TransportRequest> TransportRequests { get; set; }
    public DbSet<TransportOffer> TransportOffers { get; set; }
    public DbSet<TransportTracking> TransportTrackings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        LivestockTradingModelBuilder.Build(modelBuilder);
    }
}

public class LivestockTradingDbContextOptions
{
    public readonly DbContextOptions<LivestockTradingModuleDbContext> DbContextOptions;
    public readonly DbContextOptions<DefinitionDbContext> DefinitionDbContextOptions;

    public LivestockTradingDbContextOptions(
        RelationalDbConfiguration configuration,
        EnvironmentService environmentService,
        DefinitionDbContextOptions definitionDbContextOptions)
    {
        this.DefinitionDbContextOptions = definitionDbContextOptions.DbContextOptions;

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<LivestockTradingModuleDbContext>();

        if (environmentService.Environment == CustomEnvironments.Test)
            dbContextOptionsBuilder.UseInMemoryDatabase("globallivestock-inmemory-db");
        else
            dbContextOptionsBuilder.UseSqlServer(configuration.ConnectionString, sql =>
            {
                sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(3),
                    errorNumbersToAdd: null
                );
            });

        this.DbContextOptions = dbContextOptionsBuilder.Options;
    }
}
