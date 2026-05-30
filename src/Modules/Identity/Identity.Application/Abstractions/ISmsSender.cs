using LivestockTrading.Identity.Domain.ValueObjects;

namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Application port for transactional outbound SMS (phone OTP delivery).
/// Concrete implementation lives in Identity.Infrastructure (Wave 4 W4.3) as a
/// Faz 1 NoOp + log adapter; the real provider integration (Twilio) tracks
/// against Backlog #57/#73 inside the Notifications module (plan-doc
/// 05-identity §13 Faz 2). The port is kept even after Notifications goes live
/// so handlers stay decoupled from the transport — a future revision may swap
/// the in-process port for an event publish (UserPhoneOtpRequested) without
/// touching call-sites. IEmailSender Faz 1 stub deseni emsali.
/// </summary>
public interface ISmsSender
{
    Task SendOtpAsync(PhoneNumber phone, string code, CancellationToken ct);
}
