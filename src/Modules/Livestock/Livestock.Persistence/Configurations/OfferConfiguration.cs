using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Persistence.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OfferedPrice).HasPrecision(18, 4);
        builder.Property(x => x.CounterPrice).HasPrecision(18, 4);
        builder.Property(x => x.CurrencyCode).HasMaxLength(10);
        builder.Property(x => x.Note).HasMaxLength(1000);
        builder.Property(x => x.CounterNote).HasMaxLength(1000);

        builder.HasOne(x => x.Product).WithMany(x => x.Offers).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
