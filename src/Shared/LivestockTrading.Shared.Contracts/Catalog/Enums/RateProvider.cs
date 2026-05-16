namespace Shared.Contracts.Catalog;

/// <summary>Currency rate kaynağı (§6:710). 3-tier TCMB→ECB→exchangerate.host; Manual admin override.</summary>
public enum RateProvider { Tcmb = 1, Ecb = 2, ExchangeRateHost = 3, Manual = 99 }
