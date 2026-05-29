namespace LivestockTrading.Identity.Domain.Events.Public;

using Shared.Domain;

public sealed record UserEmailVerified(Guid UserId, string Email, DateTimeOffset VerifiedAt) : DomainEventBase;
