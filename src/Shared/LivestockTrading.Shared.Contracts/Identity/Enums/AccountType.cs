namespace Shared.Contracts.Identity;

/// <summary>Kullanıcının kayıt sırasında seçtiği hesap tipi — IMMUTABLE (v2).</summary>
public enum AccountType
{
    Producer = 1,   // Üretici — seller onboarding bekliyor
    Trader = 2,     // Tüccar — seller onboarding bekliyor
    Vet = 3,        // Veteriner — manual verification (Identity Q3)
    Buyer = 4       // Default
}
