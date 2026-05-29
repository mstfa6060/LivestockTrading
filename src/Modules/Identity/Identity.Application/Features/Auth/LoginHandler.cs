using LivestockTrading.Identity.Application.Abstractions;
using LivestockTrading.Identity.Application.Common.Mappers;
using LivestockTrading.Identity.Domain.Aggregates;
using LivestockTrading.Identity.Domain.ValueObjects;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Identity.Application.Features.Auth;

public sealed class LoginHandler : IConsumer<LoginCommand>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IRefreshTokenGenerator _rtGen;
    private readonly IJwtTokenService _jwt;
    private readonly TimeProvider _clock;

    public LoginHandler(
        IUserRepository users,
        IPasswordHasher<User> hasher,
        IRefreshTokenGenerator rtGen,
        IJwtTokenService jwt,
        TimeProvider clock)
    {
        _users = users;
        _hasher = hasher;
        _rtGen = rtGen;
        _jwt = jwt;
        _clock = clock;
    }

    public async Task Consume(ConsumeContext<LoginCommand> context)
    {
        var dto = context.Message.Dto;
        var ct = context.CancellationToken;
        var now = _clock.GetUtcNow();

        try
        {
            User? user = dto.Method switch
            {
                "email" => await _users.GetByEmailAsync(new EmailAddress(dto.Identifier), ct),
                "phone" => await _users.GetByPhoneAsync(new PhoneNumber(dto.Identifier), ct),
                "nationalId" => await _users.GetByNationalIdAsync(new NationalId(dto.Identifier), ct),
                _ => null,
            };

            if (user is null)
            {
                await Fail(context, "INVALID_CREDENTIALS", "Invalid credentials.");
                return;
            }

            if (user.Password is null)
            {
                await Fail(context, "INVALID_CREDENTIALS", "Invalid credentials.");
                return;
            }

            if (!user.Password.Value.Verify<User>(dto.Password, _hasher))
            {
                await Fail(context, "INVALID_CREDENTIALS", "Invalid credentials.");
                return;
            }

            var device = user.Devices.FirstOrDefault(d => d.DeviceFingerprint == dto.DeviceFingerprint)
                ?? user.RegisterDevice(dto.Platform, context.Message.UserAgent, dto.DeviceFingerprint, null, now);

            user.RecordLogin(device.Id, context.Message.IpAddress, now);

            var pair = _rtGen.Generate();
            var familyId = Guid.CreateVersion7();
            var ttl = dto.RememberMe ? TimeSpan.FromDays(90) : TimeSpan.FromDays(30);
            user.IssueRefreshToken(device.Id, pair.Hash, familyId, ttl, now);

            var access = await _jwt.IssueAsync(user, device.Id, ct);
            var summary = user.ToSummary();

            await context.RespondAsync<Result<LoginResponse>>(Result.Success(
                new LoginResponse(access.Value, access.ExpiresAt, pair.Raw, now.Add(ttl), summary)));
        }
        catch (DomainException ex)
        {
            await context.RespondAsync<Result<LoginResponse>>(
                Result.Failure<LoginResponse>(new Error("USER_RULE_VIOLATION", ex.Message)));
        }
    }

    private static async Task Fail(ConsumeContext<LoginCommand> context, string code, string message)
    {
        await context.RespondAsync<Result<LoginResponse>>(
            Result.Failure<LoginResponse>(new Error(code, message)));
    }
}
