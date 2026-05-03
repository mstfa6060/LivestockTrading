using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Common.Definitions.Infrastructure.RelationalDB;

public static class CommonModelBuilder
{
    public static void Build(ModelBuilder modelBuilder)
    {
        RoleRelations.Build(modelBuilder);
    }

    // SQL Server datetime/datetime2 timezone bilgisi tasimaz; EF Core okurken
    // DateTime'lari Kind=Unspecified olarak doner ve bu durumda System.Text.Json
    // serialize'i `Z` suffix'i koymaz. Frontend de timezone'u bilinmeyen string'i
    // local time olarak parse eder; Turkiye'de UTC+3 oldugu icin kayit zamanlari
    // 3 saat oynamis gorunur. Tum DateTime/DateTime? property'lerine value converter
    // uygulayip okurken Kind=Utc set ediyoruz; yazma path'inde dokunmuyoruz cunku
    // tum yazimlar zaten DateTime.UtcNow kullaniyor.
    //
    // Kullanim: en alttaki (concrete) DbContext'in OnModelCreating'inin SONUNDA
    // cagir; aksi halde child class'larda eklenen entity'lere uygulanmaz.
    public static void ApplyUtcDateTimeConverter(ModelBuilder modelBuilder)
    {
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableUtcConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(utcConverter);
                else if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(nullableUtcConverter);
            }
        }
    }
}