namespace LivestockTrading.Identity.Domain.Events.Internal;

using Shared.Domain;

public sealed record DataExportRequested(
    Guid UserId,
    string Format,
    Guid JobId,
    DateTimeOffset RequestedAt) : DomainEventBase;
