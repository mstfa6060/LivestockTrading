using Microsoft.EntityFrameworkCore;
using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Infrastructure.RelationalDB;

public interface ILivestockTradingModuleDbContext
{
    // Categories & Products
    DbSet<Category> Categories { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Brand> Brands { get; set; }
    DbSet<ProductImage> ProductImages { get; set; }
    DbSet<ProductVideo> ProductVideos { get; set; }
    DbSet<ProductDocument> ProductDocuments { get; set; }
    DbSet<ProductVariant> ProductVariants { get; set; }
    DbSet<ProductPrice> ProductPrices { get; set; }

    // Location & Sellers
    DbSet<Location> Locations { get; set; }
    DbSet<Seller> Sellers { get; set; }
    DbSet<Farm> Farms { get; set; }

    // Reviews & User Related
    DbSet<ProductReview> ProductReviews { get; set; }
    DbSet<SellerReview> SellerReviews { get; set; }
    DbSet<FavoriteProduct> FavoriteProducts { get; set; }
    DbSet<Notification> Notifications { get; set; }
    DbSet<SearchHistory> SearchHistories { get; set; }
    DbSet<ProductViewHistory> ProductViewHistories { get; set; }
    DbSet<UserPreferences> UserPreferences { get; set; }

    // Product Specific Info
    DbSet<AnimalInfo> AnimalInfos { get; set; }
    DbSet<HealthRecord> HealthRecords { get; set; }
    DbSet<Vaccination> Vaccinations { get; set; }
    DbSet<ChemicalInfo> ChemicalInfos { get; set; }
    DbSet<MachineryInfo> MachineryInfos { get; set; }
    DbSet<SeedInfo> SeedInfos { get; set; }
    DbSet<FeedInfo> FeedInfos { get; set; }
    DbSet<VeterinaryInfo> VeterinaryInfos { get; set; }

    // Messaging
    DbSet<Conversation> Conversations { get; set; }
    DbSet<Message> Messages { get; set; }
    DbSet<Offer> Offers { get; set; }

    // Helpers & Configuration
    DbSet<Currency> Currencies { get; set; }
    DbSet<PaymentMethod> PaymentMethods { get; set; }
    DbSet<ShippingCarrier> ShippingCarriers { get; set; }
    DbSet<FAQ> FAQs { get; set; }
    DbSet<Banner> Banners { get; set; }
    DbSet<Language> Languages { get; set; }
    DbSet<TaxRate> TaxRates { get; set; }
    DbSet<ShippingZone> ShippingZones { get; set; }
    DbSet<ShippingRate> ShippingRates { get; set; }

    // Deals & Transport
    DbSet<Deal> Deals { get; set; }
    DbSet<Transporter> Transporters { get; set; }
    DbSet<TransporterReview> TransporterReviews { get; set; }
    DbSet<TransportRequest> TransportRequests { get; set; }
    DbSet<TransportOffer> TransportOffers { get; set; }
    DbSet<TransportTracking> TransportTrackings { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
