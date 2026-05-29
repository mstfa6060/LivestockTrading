using Shared.Contracts.Identity;

namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// PATCH /identity/users/me/preferences — UserPreferences record is the
/// cross-module shape and the Domain property type (Shared.Contracts.Identity),
/// so the request body carries it directly (no dto-mapping). The whole record
/// is replaced atomically; partial-update semantics are not supported in Faz 1.
/// </summary>
public sealed record UpdatePreferencesCommand(Guid UserId, UserPreferences Preferences);
