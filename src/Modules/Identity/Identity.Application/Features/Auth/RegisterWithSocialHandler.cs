using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Domain.Aggregates;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class RegisterWithSocialHandler : IConsumer<RegisterWithSocialCommand>
{
    private readonly IUserRepository _users;
    private readonly IExternalLoginValidator _validator;
    private readonly IRefreshTokenGenerator _rtGen;
    private readonly IJwtTokenService _jwt;
    private readonly TimeProvider _clock;

    public RegisterWithSocialHandler(
        IUserRepository users,
        IExternalLoginValidator validator,
        IRefreshTokenGenerator rtGen,
        IJwtTokenService jwt,
        TimeProvider clock)
    {
        _users = users;
        _validator = validator;
        _rtGen = rtGen;
        _jwt = jwt;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<RegisterWithSocialCommand> context)
    {
        var dto = context.Message.Dto;
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            var validation = await _validator.ValidateAsync(dto.Provider, dto.IdToken, ct);
            if (validation.IsFailure)
            {
                await context.RespondAsync<Result<LoginResponse>>(
                    Result.Failure<LoginResponse>(validation.Error!));
                return;
            }

            var ext = validation.Value;
            var email = new EmailAddress(ext.Email);

            User user;
            var existing = await _users.GetByEmailAsync(email, ct);
            if (existing is not null)
            {
                // Auto-link akisi (plan-doc §10): email collision -> mevcut user'a
                // provider eklenir, CONFLICT degil. LinkExternalLogin idempotent.
                existing.LinkExternalLogin(dto.Provider, ext.ExternalId, now);
                user = existing;
            }
            else
            {
                var name = new PersonName(ext.FirstName, ext.LastName);
                var consents = dto.Consents
                    .Select(c => new ConsentGrant(c.Type, c.Version, c.Granted))
                    .ToList();

                user = User.RegisterWithSocial(
                    email,
                    name,
                    dto.Provider,
                    ext.ExternalId,
                    dto.AccountType,
                    dto.Preferences,
                    consents,
                    context.Message.IpAddress,
                    context.Message.UserAgent,
                    now);

                await _users.AddAsync(user, ct);
            }

            var device = user.Devices.FirstOrDefault(d => d.DeviceFingerprint == dto.DeviceFingerprint)
                ?? user.RegisterDevice(dto.Platform, context.Message.UserAgent, dto.DeviceFingerprint, null, now);

            user.RecordLogin(device.Id, context.Message.IpAddress, now);

            // Sosyal giris uzun-oturum tercih edilir: rememberMe=true varsayilan.
            // Doc explicit yok; mobile/web client sosyal-akistan sonra kullaniciyi
            // tekrar sifre/tiklamaya zorlamaz, 90-gun refresh oturumu mantikli.
            var response = await TokenIssuance.IssueLoginTokensAsync(
                user, device.Id, rememberMe: true, _rtGen, _jwt, now, ct);

            await context.RespondAsync<Result<LoginResponse>>(Result.Success(response));
        }
        catch (DomainException ex)
        {
            await context.RespondAsync<Result<LoginResponse>>(
                Result.Failure<LoginResponse>(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }
}
