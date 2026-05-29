namespace LivestockTrading.Identity.Domain.Enums;

/// <summary>PhoneVerificationTicket purpose discriminator — register flow vs password reset vs phone change vs login OTP (Faz 2).</summary>
public enum PhonePurpose
{
    Register = 1,
    ResetPassword = 2,
    ChangePhone = 3,
    LoginPhoneOtp = 4,  // Faz 2 — passwordless phone login
}
