using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.CertificationTypes;

/// <summary>
/// Command for updating an existing CertificationType reference entity.
/// Updates Name/Description translations + DisplayOrder; Code stable.
/// </summary>
public sealed record UpdateCertificationTypeCommand(
    int CertificationTypeId,
    UpdateCertificationTypeDto Dto,
    Guid ActorAdminId);
