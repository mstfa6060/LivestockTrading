namespace LivestockTrading.Identity.Domain.Events.Internal;

using Shared.Contracts.Identity;
using Shared.Domain;

public sealed record UserConsentRecorded(Guid UserId, ConsentType Type, string Version, bool Granted, string IpAddress) : DomainEventBase;
