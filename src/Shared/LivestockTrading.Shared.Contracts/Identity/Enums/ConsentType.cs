namespace Shared.Contracts.Identity;

/// <summary>KVKK onay türü — register'da zorunlu ve opsiyonel onaylar (v2).</summary>
public enum ConsentType
{
    TermsAndPrivacy = 1,        // ZORUNLU register'da
    MinistryDataShare = 2,      // ZORUNLU register'da (Faz 2 Accounts vaccine-sync)
    MarketingEmail = 3,         // OPSİYONEL, kullanıcı toggle
}
