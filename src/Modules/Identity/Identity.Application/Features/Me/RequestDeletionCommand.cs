namespace LivestockTrading.Identity.Application.Features.Me;

/// <summary>
/// POST /identity/users/me/account/delete-request — schedules the user for
/// deletion 30 days out (Domain RequestDeletion). The grace window can be
/// cancelled via /me/account/restore until the worker finalises deletion.
/// </summary>
public sealed record RequestDeletionCommand(Guid UserId);
