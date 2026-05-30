namespace LivestockTrading.Identity.Domain.Enums;

/// <summary>EmailVerificationTicket purpose discriminator — register/login email-verify vs password-reset (W4.2.D2-out cift-kanal Flow A, c-light).</summary>
public enum EmailPurpose
{
    Verify = 1,
    ResetPassword = 2,
}
