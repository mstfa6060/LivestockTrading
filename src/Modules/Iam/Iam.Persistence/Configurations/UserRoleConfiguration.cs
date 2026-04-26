using Iam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Iam.Persistence.Configurations;

internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasKey(ur => ur.Id);

        builder.HasIndex(ur => new { ur.UserId, ur.RoleId, ur.ModuleId }).IsUnique();

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        builder.HasOne(ur => ur.Module)
            .WithMany(m => m.UserRoles)
            .HasForeignKey(ur => ur.ModuleId);
    }
}
