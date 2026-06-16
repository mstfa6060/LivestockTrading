using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Persistence.Configurations;

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AgreePrice).HasPrecision(18, 4);
        builder.Property(x => x.CurrencyCode).HasMaxLength(10);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.CancellationReason).HasMaxLength(1000);

        builder.HasOne(x => x.Offer).WithOne(x => x.Deal).HasForeignKey<Deal>(x => x.OfferId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
