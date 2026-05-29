namespace LivestockTrading.Identity.Domain.Events.Public;

using Shared.Domain;

public sealed record UserRegistered(Guid UserId, string Email, string Locale, string Currency, string CountryCode) : DomainEventBase;
