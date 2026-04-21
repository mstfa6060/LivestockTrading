using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Persistence;

public class LivestockDbContext(DbContextOptions<LivestockDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Banner> Banners => Set<Banner>();
    public DbSet<Faq> Faqs => Set<Faq>();

    public DbSet<Seller> Sellers => Set<Seller>();
    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<Transporter> Transporters => Set<Transporter>();

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<FavoriteProduct> FavoriteProducts => Set<FavoriteProduct>();
    public DbSet<ProductReport> ProductReports => Set<ProductReport>();
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
    public DbSet<SellerReview> SellerReviews => Set<SellerReview>();
    public DbSet<TransporterReview> TransporterReviews => Set<TransporterReview>();

    public DbSet<AnimalInfo> AnimalInfos => Set<AnimalInfo>();
    public DbSet<HealthRecord> HealthRecords => Set<HealthRecord>();
    public DbSet<Vaccination> Vaccinations => Set<Vaccination>();
    public DbSet<FeedInfo> FeedInfos => Set<FeedInfo>();
    public DbSet<ChemicalInfo> ChemicalInfos => Set<ChemicalInfo>();
    public DbSet<MachineryInfo> MachineryInfos => Set<MachineryInfo>();
    public DbSet<SeedInfo> SeedInfos => Set<SeedInfo>();
    public DbSet<VeterinaryInfo> VeterinaryInfos => Set<VeterinaryInfo>();

    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();

    public DbSet<TransportRequest> TransportRequests => Set<TransportRequest>();
    public DbSet<TransportOffer> TransportOffers => Set<TransportOffer>();
    public DbSet<TransportTracking> TransportTrackings => Set<TransportTracking>();

    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<SellerSubscription> SellerSubscriptions => Set<SellerSubscription>();
    public DbSet<BoostPackage> BoostPackages => Set<BoostPackage>();
    public DbSet<ProductBoost> ProductBoosts => Set<ProductBoost>();
    public DbSet<IAPTransaction> IAPTransactions => Set<IAPTransaction>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AppVersionConfig> AppVersionConfigs => Set<AppVersionConfig>();

    public DbSet<ProductViewHistory> ProductViewHistories => Set<ProductViewHistory>();
    public DbSet<SearchHistory> SearchHistories => Set<SearchHistory>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();

    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductPrice> ProductPrices => Set<ProductPrice>();

    public DbSet<ShippingCarrier> ShippingCarriers => Set<ShippingCarrier>();
    public DbSet<ShippingZone> ShippingZones => Set<ShippingZone>();
    public DbSet<ShippingRate> ShippingRates => Set<ShippingRate>();

    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();
    public DbSet<ContactForm> ContactForms => Set<ContactForm>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LivestockDbContext).Assembly);
    }
}
