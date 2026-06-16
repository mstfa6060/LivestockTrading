using Iam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Iam.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    private static readonly Guid AdminId = new("a1000000-0000-0000-0000-000000000001");
    private static readonly Guid ModeratorId = new("a1000000-0000-0000-0000-000000000002");
    private static readonly Guid SellerId = new("a1000000-0000-0000-0000-000000000003");
    private static readonly Guid TransporterId = new("a1000000-0000-0000-0000-000000000004");
    private static readonly Guid BuyerId = new("a1000000-0000-0000-0000-000000000006");

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        builder.Property(r => r.Description).HasMaxLength(500);

        builder.HasIndex(r => r.Name).IsUnique();

        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new Role { Id = AdminId, Name = "Admin", Description = "Tam yetki, tüm işlemler", CreatedAt = seedDate },
            new Role { Id = ModeratorId, Name = "Moderator", Description = "İçerik moderasyonu, onay/red", CreatedAt = seedDate },
            new Role { Id = SellerId, Name = "Seller", Description = "Ürün satışı, çiftlik yönetimi", CreatedAt = seedDate },
            new Role { Id = TransporterId, Name = "Transporter", Description = "Nakliye hizmetleri", CreatedAt = seedDate },
            new Role { Id = BuyerId, Name = "Buyer", Description = "Ürün satın alma (varsayılan rol)", CreatedAt = seedDate }
        );
    }
}
