using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Persistence.Configurations;

public class SellerSubscriptionConfiguration : IEntityTypeConfiguration<SellerSubscription>
{
    public void Configure(EntityTypeBuilder<SellerSubscription> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 4);
        builder.Property(x => x.CurrencyCode).HasMaxLength(10);
        builder.Property(x => x.ExternalSubscriptionId).HasMaxLength(300);

        builder.HasOne(x => x.Plan).WithMany(x => x.Subscriptions).HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
