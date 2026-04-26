using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(500);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.Description).HasMaxLength(10000);
        builder.Property(x => x.Price).HasPrecision(18, 4);
        builder.Property(x => x.CurrencyCode).HasMaxLength(10);
        builder.Property(x => x.Unit).HasMaxLength(50);
        builder.Property(x => x.RejectionReason).HasMaxLength(1000);

        builder.HasOne(x => x.Seller).WithMany(x => x.Products).HasForeignKey(x => x.SellerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Brand).WithMany(x => x.Products).HasForeignKey(x => x.BrandId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.Farm).WithMany(x => x.Products).HasForeignKey(x => x.FarmId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.AnimalInfo).WithOne(x => x.Product).HasForeignKey<AnimalInfo>(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.FeedInfo).WithOne(x => x.Product).HasForeignKey<FeedInfo>(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ChemicalInfo).WithOne(x => x.Product).HasForeignKey<ChemicalInfo>(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.MachineryInfo).WithOne(x => x.Product).HasForeignKey<MachineryInfo>(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SeedInfo).WithOne(x => x.Product).HasForeignKey<SeedInfo>(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.VeterinaryInfo).WithOne(x => x.Product).HasForeignKey<VeterinaryInfo>(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
