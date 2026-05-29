namespace LivestockTrading.Identity.Domain.Events.Internal;

using LivestockTrading.Identity.Domain.Enums;
using Shared.Domain;

public sealed record RefreshTokenRevoked(Guid UserId, Guid TokenId, Guid FamilyId, RevocationReason Reason) : DomainEventBase;
