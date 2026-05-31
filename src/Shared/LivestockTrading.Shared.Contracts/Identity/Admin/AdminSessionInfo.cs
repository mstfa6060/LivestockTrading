namespace Shared.Contracts.Identity.Admin;

/// <summary>
/// Admin paneli kullanici session projeksiyonu (GET /admin/users/{id}/sessions).
/// SessionInfo (Me-scope, Identity.Application/Features/Me) emsali ama Admin
/// scope'unda IsCurrent semantigi yok (admin baska oturumlari inceler). 7 field
/// — 05-identity §12 satir 597 endpoint literal, projeksiyon doc-silent oldugu
/// icin Me SessionInfo'dan IsCurrent dusurulerek tureti (B-W4.2-E-1 Karar (c)).
/// </summary>
public sealed record AdminSessionInfo(
    Guid SessionId,
    Guid DeviceId,
    string Platform,
    string? UserAgent,
    string IpAddress,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt);
