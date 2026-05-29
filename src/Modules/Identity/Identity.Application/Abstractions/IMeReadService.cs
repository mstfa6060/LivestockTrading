using LivestockTrading.Identity.Application.Features.Me;

namespace LivestockTrading.Identity.Application.Abstractions;

/// <summary>
/// Internal read port for the self-service /me/* surface. Distinct from
/// Shared.Contracts.Identity.IIdentityReadService (cross-module, cached
/// projections for Listings/Marketplace/Messaging/Notifications consumers).
/// Concrete implementation lives in Identity.Infrastructure (Wave 4 W4.3) as
/// EF projection. Endpoints consume this port directly without going through
/// MassTransit mediator (read-only, no UoW or validation pipeline needed).
/// </summary>
public interface IMeReadService
{
    Task<MeProfile?> GetMyProfileAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<SessionInfo>> GetMySessionsAsync(Guid userId, CancellationToken ct);
}
