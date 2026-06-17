using Iam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Iam.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Surname).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).HasMaxLength(512);
        builder.Property(u => u.PasswordSalt).HasMaxLength(512);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.LastOtpCode).HasMaxLength(10);
        builder.Property(u => u.City).HasMaxLength(100);
        builder.Property(u => u.District).HasMaxLength(100);
        builder.Property(u => u.Language).HasMaxLength(10);
        builder.Property(u => u.PreferredCurrencyCode).HasMaxLength(10);
        builder.Property(u => u.AuthProvider).HasMaxLength(50);
        builder.Property(u => u.ProviderKey).HasMaxLength(256);
        builder.Property(u => u.Description).HasMaxLength(500);
        builder.Property(u => u.PasswordResetToken).HasMaxLength(512);
        builder.Property(u => u.UserSource).HasConversion<int>();

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.UserName).IsUnique();

        builder.HasOne(u => u.Country)
            .WithMany()
            .HasForeignKey(u => u.CountryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
