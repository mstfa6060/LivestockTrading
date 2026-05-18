namespace LivestockTrading.Catalog.Application.Features.CertificationTypes;

/// <summary>
/// Command for deactivating a CertificationType reference entity (soft toggle).
/// Id-only; no DTO body. Port DeactivateCertificationTypeAsync — doc §8 DELETE
/// port-baskın deactivate'e map (KAYDET-14; repo Remove yok, FK guard).
/// </summary>
public sealed record DeactivateCertificationTypeCommand(
    int CertificationTypeId,
    Guid ActorAdminId);
