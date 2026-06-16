using Iam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Iam.Persistence.Configurations;

internal sealed class UserPushTokenConfiguration : IEntityTypeConfiguration<UserPushToken>
{
    public void Configure(EntityTypeBuilder<UserPushToken> builder)
    {
        builder.ToTable("UserPushTokens");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.DeviceId).IsRequired().HasMaxLength(200);
        builder.Property(t => t.PushToken).IsRequired().HasMaxLength(1024);
        builder.Property(t => t.AppName).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Platform).HasConversion<int>();

        builder.HasIndex(t => new { t.UserId, t.DeviceId, t.AppName }).IsUnique();
        builder.HasIndex(t => t.UserId);

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
