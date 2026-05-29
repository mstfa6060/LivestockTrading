using LivestockTrading.Identity.Domain.ValueObjects;

namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Application port for transactional outbound email. Concrete implementation
/// lives in Identity.Infrastructure (Wave 4 W4.3) as a Faz 1 NoOp + log adapter;
/// the real provider integration tracks against Backlog #60 (MailKit + Brevo +
/// retry + delivery feedback) inside the Notifications module. The port is kept
/// even after Notifications goes live so handlers stay decoupled from the
/// transport — a future revision may swap the in-process port for an event
/// publish (UserEmailVerificationRequested) without touching call-sites.
/// </summary>
public interface IEmailSender
{
    Task SendEmailVerificationAsync(EmailAddress to, string verifyToken, CancellationToken ct);
}
