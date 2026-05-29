namespace LivestockTrading.Identity.Domain.Entities;

using Shared.Domain;

/// <summary>
/// External login link (Google/Apple). Immutable - sadece olusturulur, modifiye edilmez.
/// Plan-doc 05-identity §3 satir 251-265 birebir.
/// </summary>
public sealed class UserExternalLogin : Entity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Provider { get; private set; }
    public string ExternalId { get; private set; }
    public DateTimeOffset LinkedAt { get; private set; }

    // EF Core
    private UserExternalLogin()
    {
        Provider = null!;
        ExternalId = null!;
    }

    // Sadece User AR cagirir - internal.
    internal UserExternalLogin(
        Guid userId,
        string provider,
        string externalId,
        DateTimeOffset linkedAt)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        Provider = provider;
        ExternalId = externalId;
        LinkedAt = linkedAt;
    }

    protected override object IdentityValue => Id;
    protected override bool IsTransient => Id == Guid.Empty;
}
