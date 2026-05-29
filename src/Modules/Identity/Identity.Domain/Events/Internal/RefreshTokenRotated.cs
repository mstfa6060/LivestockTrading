namespace LivestockTrading.Identity.Domain.Events.Internal;

using Shared.Domain;

public sealed record RefreshTokenRotated(Guid UserId, Guid OldTokenId, Guid NewTokenId, Guid FamilyId, Guid DeviceId) : DomainEventBase;
