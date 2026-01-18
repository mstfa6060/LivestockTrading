using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Infrastructure.RelationalDB;

public static class LivestockTradingModelBuilder
{
    public static void Build(ModelBuilder modelBuilder)
    {
        // ========================================
        // ENTITY CONFIGURATIONS
        // ========================================
        // Entity konfigurasyonlari buraya eklenecek
        // Ornek:
        // modelBuilder.Entity<Animal>()
        //     .HasKey(a => a.Id);
        //
        // modelBuilder.Entity<Animal>()
        //     .Property(a => a.Name)
        //     .IsRequired()
        //     .HasMaxLength(200);
    }
}
