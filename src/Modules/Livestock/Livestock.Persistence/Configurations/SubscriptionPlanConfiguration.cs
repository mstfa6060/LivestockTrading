using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Persistence.Configurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Price).HasPrecision(18, 4);
        builder.Property(x => x.CurrencyCode).HasMaxLength(10);
        builder.Property(x => x.AppStoreProductId).HasMaxLength(200);
        builder.Property(x => x.PlayStoreProductId).HasMaxLength(200);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
