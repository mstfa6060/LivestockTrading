namespace Shared.Contracts.Catalog;

/// <summary>
/// Currency rate kaynağı (§6:710). 3-tier TCMB→ECB→Fawazahmed0 currency-api; Manual admin override.
/// W3.6.B Frontend kararı: ExchangeRateHost (plan-doc §6:635, 2025) → CurrencyApi rename (aynı int 3),
/// sebep: apilayer akquisition sonrası fiili 2026-05 API key paywall (B.4 SENARYO-2 yakalama, Aile 2).
/// Plan-doc §6:635 revize Wave 3 sonu doc-finalize commit'ine ertelendi.
/// </summary>
public enum RateProvider { Tcmb = 1, Ecb = 2, CurrencyApi = 3, Manual = 99 }
