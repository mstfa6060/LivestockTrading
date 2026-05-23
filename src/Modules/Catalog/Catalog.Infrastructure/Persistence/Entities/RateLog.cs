using System.Text.Json;
using LivestockTrading.Catalog.Application.Abstractions;
using Shared.Contracts.Catalog;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Entities;

/// <summary>
/// Currency-rate fetch audit log. Infra-local POCO — Domain'e GİRMEZ (Karar 5, Wave 1
/// invariant); Shared.Domain.Entity base'i KULLANMAZ (identity-equality / domain-event
/// gereği yok, append-only kayıt). Doc 05-catalog.md §6:702-713 grounded:
/// Id/RateDate/Source/RatesJson/Success/Error/FetchedAt. RateProvider = mevcut
/// Shared.Contracts.Catalog enum (Adım 1.e teyitli, yeniden tanımlanmaz).
/// Append-only: mutator metot yok; retention/cleanup W3.6 Quartz (§6:716).
/// Tablo: catalog.rate_logs (RateLogConfiguration).
/// </summary>
public sealed class RateLog
{
    public Guid Id { get; private set; }
    public DateOnly RateDate { get; private set; }
    public RateProvider Source { get; private set; }
    public string RatesJson { get; private set; }
    public bool Success { get; private set; }
    public string? Error { get; private set; }
    public DateTime FetchedAt { get; private set; }

    // EF Core materialization
    private RateLog()
    {
        RatesJson = null!;
    }

    public RateLog(
        DateOnly rateDate,
        RateProvider source,
        string ratesJson,
        bool success,
        string? error,
        DateTime fetchedAt)
    {
        Id = Guid.CreateVersion7();   // zaman-sıralı, FetchedAt ile tutarlı
        RateDate = rateDate;
        Source = source;
        RatesJson = ratesJson;
        Success = success;
        Error = error;
        FetchedAt = fetchedAt;
    }

    /// <summary>
    /// W3.6.C Cron Job + W3.6.D Handler kullanim factory (plan-doc 05-catalog.md §6:669).
    /// 3-tier IRateProvider chain sonucunu RateLog audit kaydina cevirir.
    /// Id atama ctor icinde Guid.CreateVersion7() (zaman-sirali, W3.1 entity invariant korunur).
    /// FetchedAt = DateTime.UtcNow (UTC audit invariant).
    /// JSON serialize factory icinde kapsullenir (caller decimal Dictionary verir, plain string store edilir).
    /// </summary>
    public static RateLog Create(RateProvider source, RateFetchResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var json = JsonSerializer.Serialize(result.RatesToUsd);

        return new RateLog(
            rateDate: result.RateDate,
            source: source,
            ratesJson: json,
            success: result.Success,
            error: result.Error,
            fetchedAt: DateTime.UtcNow);
    }
}
