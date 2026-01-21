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
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductVideo> ProductVideos { get; set; }
    public DbSet<ProductDocument> ProductDocuments { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductPrice> ProductPrices { get; set; }

    // Location & Sellers
    public DbSet<Location> Locations { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Farm> Farms { get; set; }

    // Orders & Cart
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<OrderPayment> OrderPayments { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

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
    public new DbSet<Country> Countries { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<ShippingCarrier> ShippingCarriers { get; set; }
    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<TaxRate> TaxRates { get; set; }
    public DbSet<ShippingZone> ShippingZones { get; set; }
    public DbSet<ShippingRate> ShippingRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        LivestockTradingModelBuilder.Build(modelBuilder);

        // Prefix all LivestockTrading tables to avoid collisions with other modules
        var prefix = "LivestockTrading_";
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType == null)
                continue;

            var ns = clrType.Namespace ?? string.Empty;
            if (!ns.StartsWith("LivestockTrading.Domain"))
                continue;

            var currentName = entityType.GetTableName();
            if (string.IsNullOrWhiteSpace(currentName))
                continue;

            if (!currentName.StartsWith(prefix, StringComparison.Ordinal))
            {
                entityType.SetTableName(prefix + currentName);
            }
        }
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
