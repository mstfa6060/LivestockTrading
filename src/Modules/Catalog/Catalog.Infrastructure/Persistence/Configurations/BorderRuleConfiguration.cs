using LivestockTrading.Catalog.Domain.Aggregates;
using LivestockTrading.Catalog.Domain.Entities;
using LivestockTrading.Catalog.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// BorderRule AR mapping (v2, Faz 2 feature). Kural 5:362 + Adım 2 tasarım. PK Guid v7
/// app-assigned (BorderRule.cs:50 → ValueGeneratedNever). From/ToCountry CountryCode
/// non-null converter (Revize 3). CategoryId?/BreedId? nullable FK navigationsız (Kural 1).
/// Kind enum → int. RestrictionsJson jsonb opaque Faz 1 (Açık Karar 2). Notes → jsonb.
/// </summary>
public sealed class BorderRuleConfiguration : IEntityTypeConfiguration<BorderRule>
{
    public void Configure(EntityTypeBuilder<BorderRule> builder)
    {
        builder.ToTable("border_rules");
        builder.HasKey(br => br.Id);
        builder.Property(br => br.Id).ValueGeneratedNever();

        builder.Property(br => br.Kind).HasConversion<int>().IsRequired();
        builder.Property(br => br.RestrictionsJson)
            .IsRequired()
            .HasColumnType("jsonb");
        builder.Property(br => br.EffectiveFrom);
        builder.Property(br => br.EffectiveUntil);
        builder.Property(br => br.IsActive).IsRequired();
        builder.Property(br => br.CreatedByUserId).IsRequired();
        builder.Property(br => br.CreatedAt).IsRequired();
        builder.Property(br => br.UpdatedAt).IsRequired();

        builder.Property(br => br.FromCountry)
            .HasConversion(CountryCodeConverter.NonNull)
            .HasMaxLength(2)
            .IsRequired();
        builder.Property(br => br.ToCountry)
            .HasConversion(CountryCodeConverter.NonNull)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(br => br.Notes)
            .HasConversion(new TranslationsToJsonConverter(), TranslationsToJsonConverter.Comparer)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(br => br.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Breed>()
            .WithMany()
            .HasForeignKey(br => br.BreedId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(br => new { br.FromCountry, br.ToCountry });
    }
}
