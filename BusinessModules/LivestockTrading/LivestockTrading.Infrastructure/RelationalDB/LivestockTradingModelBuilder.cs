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
            entity.HasIndex(e => e.Slug);
            entity.Property(e => e.BasePrice).HasPrecision(18, 2);
            entity.Property(e => e.DiscountedPrice).HasPrecision(18, 2);

            entity.HasOne(e => e.Category)
                .WithMany(e => e.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Seller)
                .WithMany(e => e.Products)
                .HasForeignKey(e => e.SellerId)
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
            entity.Property(e => e.Latitude).HasPrecision(10, 7);
            entity.Property(e => e.Longitude).HasPrecision(10, 7);
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
        // ORDER
        // ========================================
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);

            entity.HasOne(e => e.ShippingAddress)
                .WithMany(e => e.OrderShippingAddresses)
                .HasForeignKey(e => e.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.BillingAddress)
                .WithMany(e => e.OrderBillingAddresses)
                .HasForeignKey(e => e.BillingAddressId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ========================================
        // CART
        // ========================================
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.SessionId);
        });

        // ========================================
        // PRODUCT MEDIA
        // ========================================
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Product)
                .WithMany(e => e.Images)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductVideo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Product)
                .WithMany(e => e.Videos)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
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
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.DiscountedPrice).HasPrecision(18, 2);
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // TAX & SHIPPING
        // ========================================
        modelBuilder.Entity<TaxRate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Rate).HasPrecision(5, 2);
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
            entity.Property(e => e.ShippingCost).HasPrecision(18, 2);
            entity.Property(e => e.MinWeight).HasPrecision(10, 2);
            entity.Property(e => e.MaxWeight).HasPrecision(10, 2);
            entity.Property(e => e.MinOrderAmount).HasPrecision(18, 2);

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
        // COUNTRY
        // ========================================
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(3);
            entity.HasIndex(e => e.Code).IsUnique();
        });
    }
}
