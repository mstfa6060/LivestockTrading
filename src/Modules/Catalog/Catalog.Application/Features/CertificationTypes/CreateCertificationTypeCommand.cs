using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.CertificationTypes;

/// <summary>
/// Command for creating a new CertificationType reference entity (admin).
/// DescriptionTranslations nullable DTO → handler Translations.Empty coalesce (F-S5).
/// </summary>
public sealed record CreateCertificationTypeCommand(
    CreateCertificationTypeDto Dto,
    Guid ActorAdminId);
