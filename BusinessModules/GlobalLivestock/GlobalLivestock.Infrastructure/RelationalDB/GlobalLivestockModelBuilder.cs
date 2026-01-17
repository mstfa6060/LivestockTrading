using Microsoft.EntityFrameworkCore;

namespace GlobalLivestock.Infrastructure.RelationalDB;

public static class GlobalLivestockModelBuilder
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
