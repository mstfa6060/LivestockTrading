using Iam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Iam.Persistence.Configurations;

internal sealed class IamModuleConfiguration : IEntityTypeConfiguration<IamModule>
{
    private static readonly Guid LivestockTradingModuleId = new("DFD018C9-FC32-42C4-AEFD-70A5942A295E");

    public void Configure(EntityTypeBuilder<IamModule> builder)
    {
        builder.ToTable("Modules");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name).IsRequired().HasMaxLength(100);

        builder.HasIndex(m => m.Name).IsUnique();

        builder.HasData(
            new IamModule { Id = LivestockTradingModuleId, Name = "LivestockTrading" }
        );
    }
}
