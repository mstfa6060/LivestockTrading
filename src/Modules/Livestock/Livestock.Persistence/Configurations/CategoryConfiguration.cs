using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(200);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.IconUrl).HasMaxLength(500);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Property(x => x.NameTranslations).HasColumnType("jsonb");
        builder.Property(x => x.DescriptionTranslations).HasColumnType("jsonb");

        builder.HasOne(x => x.ParentCategory)
            .WithMany(x => x.SubCategories)
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
