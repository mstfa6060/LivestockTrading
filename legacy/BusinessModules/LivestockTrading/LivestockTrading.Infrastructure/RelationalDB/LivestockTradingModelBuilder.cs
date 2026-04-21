using Microsoft.EntityFrameworkCore;
using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Infrastructure.RelationalDB;

public static class LivestockTradingModelBuilder
{
    public static void Build(ModelBuilder modelBuilder)
    {
        // ========================================
        // CATEGORY
        // ========================================
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Slug).IsUnique();

            entity.HasOne(e => e.ParentCategory)
                .WithMany(e => e.SubCategories)
                .HasForeignKey(e => e.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ========================================
        // PRODUCT
        // ========================================
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.Slug).IsUnique();

            entity.HasOne(e => e.Category)
                .WithMany(e => e.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Seller)
                .WithMany(e => e.Products)
                .HasForeignKey(e => e.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Brand)
                .WithMany(e => e.Products)
                .HasForeignKey(e => e.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Location)
                .WithMany(e => e.Products)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ========================================
        // BRAND
        // ========================================
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Name);
        });

        // ========================================
        // LOCATION
        // ========================================
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // ========================================
        // SELLER
        // ========================================
        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BusinessName).IsRequired().HasMaxLength(300);
            entity.HasIndex(e => e.UserId);
        });

        // ========================================
        // FARM
        // ========================================
        modelBuilder.Entity<Farm>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(300);

            entity.HasOne(e => e.Seller)
                .WithMany(e => e.Farms)
                .HasForeignKey(e => e.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Location)
                .WithMany(e => e.Farms)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ========================================
        // PRODUCT VARIANT
        // ========================================
        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                .WithMany(e => e.Variants)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // PRODUCT REVIEW
        // ========================================
        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);

            entity.HasOne(e => e.Product)
                .WithMany(e => e.Reviews)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // SELLER REVIEW
        // ========================================
        modelBuilder.Entity<SellerReview>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);

            entity.HasOne(e => e.Seller)
                .WithMany(e => e.Reviews)
                .HasForeignKey(e => e.SellerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // FAVORITE PRODUCT
        // ========================================
        modelBuilder.Entity<FavoriteProduct>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);

            entity.HasOne(e => e.Product)
                .WithMany(e => e.Favorites)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // PRODUCT VIEW HISTORY
        // ========================================
        modelBuilder.Entity<ProductViewHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // NOTIFICATION
        // ========================================
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
        });

        // ========================================
        // SEARCH HISTORY
        // ========================================
        modelBuilder.Entity<SearchHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
        });

        // ========================================
        // USER PREFERENCES
        // ========================================
        modelBuilder.Entity<UserPreferences>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
        });

        // ========================================
        // MULTILINGUAL & MULTI-CURRENCY
        // ========================================
        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(3);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<ProductPrice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // PRODUCT IMAGE
        // ========================================
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.ProductId);
        });

        // ========================================
        // TAX & SHIPPING
        // ========================================
        modelBuilder.Entity<TaxRate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CountryCode).HasMaxLength(3);
            entity.Property(e => e.StateCode).HasMaxLength(10);
        });

        modelBuilder.Entity<ShippingZone>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

            entity.HasOne(e => e.Seller)
                .WithMany()
                .HasForeignKey(e => e.SellerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ShippingRate>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.ShippingZone)
                .WithMany(e => e.ShippingRates)
                .HasForeignKey(e => e.ShippingZoneId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ShippingCarrier)
                .WithMany()
                .HasForeignKey(e => e.ShippingCarrierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ========================================
        // SUBSCRIPTION PLAN
        // ========================================
        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.AppleProductIdMonthly).HasMaxLength(200);
            entity.Property(e => e.AppleProductIdYearly).HasMaxLength(200);
            entity.Property(e => e.GoogleProductIdMonthly).HasMaxLength(200);
            entity.Property(e => e.GoogleProductIdYearly).HasMaxLength(200);
        });

        // ========================================
        // SELLER SUBSCRIPTION
        // ========================================
        modelBuilder.Entity<SellerSubscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OriginalTransactionId).HasMaxLength(500);

            entity.HasOne(e => e.Seller)
                .WithMany()
                .HasForeignKey(e => e.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SubscriptionPlan)
                .WithMany(e => e.SellerSubscriptions)
                .HasForeignKey(e => e.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.SellerId);
            entity.HasIndex(e => e.Status);
        });

        // ========================================
        // IAP TRANSACTION
        // ========================================
        modelBuilder.Entity<IAPTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StoreTransactionId).HasMaxLength(500);
            entity.Property(e => e.Currency).HasMaxLength(3);

            entity.HasOne(e => e.SellerSubscription)
                .WithMany()
                .HasForeignKey(e => e.SellerSubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.StoreTransactionId);
        });

        // ========================================
        // BOOST PACKAGE
        // ========================================
        modelBuilder.Entity<BoostPackage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.AppleProductId).HasMaxLength(200);
            entity.Property(e => e.GoogleProductId).HasMaxLength(200);
        });

        // ========================================
        // PRODUCT BOOST
        // ========================================
        modelBuilder.Entity<ProductBoost>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Seller)
                .WithMany()
                .HasForeignKey(e => e.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.BoostPackage)
                .WithMany()
                .HasForeignKey(e => e.BoostPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.IAPTransaction)
                .WithMany()
                .HasForeignKey(e => e.IAPTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.ExpiresAt);
        });

        // ========================================
        // SELLER - Active Subscription Navigation
        // ========================================
        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasOne(e => e.ActiveSubscription)
                .WithMany()
                .HasForeignKey(e => e.ActiveSubscriptionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Country is managed by DefinitionDbContext (base class)
        modelBuilder.Ignore<Country>();

        // ========================================
        // APP VERSION CONFIG
        // ========================================
        modelBuilder.Entity<AppVersionConfig>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MinSupportedVersion).HasMaxLength(20).IsRequired();
            entity.Property(e => e.LatestVersion).HasMaxLength(20).IsRequired();
            entity.Property(e => e.StoreUrl).HasMaxLength(500);
            entity.Property(e => e.UpdateMessage).HasMaxLength(500);
            entity.HasIndex(e => e.Platform);
        });

        // ========================================
        // SHIPPING CARRIER
        // ========================================
        modelBuilder.Entity<ShippingCarrier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.HasIndex(e => e.Code);
        });

        // ========================================
        // PAYMENT METHOD
        // ========================================
        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // ========================================
        // FAQ
        // ========================================
        modelBuilder.Entity<FAQ>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // BANNER
        // ========================================
        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        // ========================================
        // DEAL (Alıcı-Satıcı Anlaşması)
        // ========================================
        modelBuilder.Entity<Deal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DealNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.DealNumber).IsUnique();
            entity.Property(e => e.Currency).HasMaxLength(3);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Seller)
                .WithMany()
                .HasForeignKey(e => e.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TransportRequest)
                .WithMany()
                .HasForeignKey(e => e.TransportRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.BuyerId);
            entity.HasIndex(e => e.Status);
        });

        // ========================================
        // TRANSPORTER (Nakliyeci)
        // ========================================
        modelBuilder.Entity<Transporter>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.CountryCode).HasMaxLength(3);

            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.IsVerified);
            entity.HasIndex(e => e.IsActive);
        });

        // ========================================
        // TRANSPORTER REVIEW
        // ========================================
        modelBuilder.Entity<TransporterReview>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Transporter)
                .WithMany(e => e.Reviews)
                .HasForeignKey(e => e.TransporterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.TransportRequest)
                .WithMany()
                .HasForeignKey(e => e.TransportRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.UserId);
        });

        // ========================================
        // TRANSPORT REQUEST (Nakliye Talebi)
        // ========================================
        modelBuilder.Entity<TransportRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Currency).HasMaxLength(3);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Seller)
                .WithMany()
                .HasForeignKey(e => e.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PickupLocation)
                .WithMany()
                .HasForeignKey(e => e.PickupLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeliveryLocation)
                .WithMany()
                .HasForeignKey(e => e.DeliveryLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedTransporter)
                .WithMany(e => e.AssignedTransports)
                .HasForeignKey(e => e.AssignedTransporterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.BuyerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsInPool);
        });

        // ========================================
        // TRANSPORT OFFER (Nakliyeci Teklifi)
        // ========================================
        modelBuilder.Entity<TransportOffer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Currency).HasMaxLength(3);

            entity.HasOne(e => e.TransportRequest)
                .WithMany(e => e.TransportOffers)
                .HasForeignKey(e => e.TransportRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Transporter)
                .WithMany(e => e.TransportOffers)
                .HasForeignKey(e => e.TransporterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Status);
        });

        // ========================================
        // TRANSPORT TRACKING (Nakliye Takip)
        // ========================================
        modelBuilder.Entity<TransportTracking>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.TransportRequest)
                .WithMany(e => e.TrackingHistory)
                .HasForeignKey(e => e.TransportRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.RecordedAt);
        });

        // ========================================
        // ANIMAL INFO
        // ========================================
        modelBuilder.Entity<AnimalInfo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // HEALTH RECORD
        // ========================================
        modelBuilder.Entity<HealthRecord>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.AnimalInfo)
                .WithMany(e => e.HealthRecords)
                .HasForeignKey(e => e.AnimalInfoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // VACCINATION
        // ========================================
        modelBuilder.Entity<Vaccination>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.AnimalInfo)
                .WithMany(e => e.Vaccinations)
                .HasForeignKey(e => e.AnimalInfoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // CHEMICAL INFO
        // ========================================
        modelBuilder.Entity<ChemicalInfo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // MACHINERY INFO
        // ========================================
        modelBuilder.Entity<MachineryInfo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // SEED INFO
        // ========================================
        modelBuilder.Entity<SeedInfo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // FEED INFO
        // ========================================
        modelBuilder.Entity<FeedInfo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // VETERINARY INFO
        // ========================================
        modelBuilder.Entity<VeterinaryInfo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // CONVERSATION
        // ========================================
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ParticipantUserId1);
            entity.HasIndex(e => e.ParticipantUserId2);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ========================================
        // MESSAGE
        // ========================================
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SenderUserId);
            entity.HasIndex(e => e.RecipientUserId);

            entity.HasOne(e => e.Conversation)
                .WithMany(e => e.Messages)
                .HasForeignKey(e => e.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // OFFER
        // ========================================
        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BuyerUserId);
            entity.HasIndex(e => e.SellerUserId);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CounterOfferTo)
                .WithMany()
                .HasForeignKey(e => e.CounterOfferToId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
