using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Persistence.Configurations;

public class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BusinessName).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Description).HasMaxLength(3000);
        builder.Property(x => x.PhoneNumber).HasMaxLength(30);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.WebsiteUrl).HasMaxLength(500);
        builder.Property(x => x.TaxNumber).HasMaxLength(100);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
        builder.Property(x => x.SuspensionReason).HasMaxLength(1000);
        builder.HasIndex(x => x.UserId).IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
