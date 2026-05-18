using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.Locations;
using LivestockTrading.Catalog.Domain.Entities;
using MassTransit;
using NSubstitute;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;
using Shared.ValueObjects;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.Locations;

public class CreateLocationHandlerTests
{
    private readonly IReferenceDataRepository _repo = Substitute.For<IReferenceDataRepository>();
    private readonly CreateLocationHandler _handler;

    public CreateLocationHandlerTests() => _handler = new CreateLocationHandler(_repo);

    private static ConsumeContext<CreateLocationCommand> Context(CreateLocationCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<CreateLocationCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static Location ParentLocation() =>
        new(null, LocationLevel.Country, new CountryCode("tr"), "tr", "tr", "tr",
            LocationTestHelpers.EnglishText("Turkey"), null, null, 0, 0);

    private static CreateLocationDto Dto(
        int? parentId, Shared.Contracts.Catalog.LocationLevel level, string countryCode, string code) =>
        new(parentId, level, countryCode, code,
            LocationTestHelpers.EnglishText("Istanbul"), null, 0, 0);

    [Fact]
    public async Task Consume_WhenRootLocation_CreatesLocation_RespondsSuccess()
    {
        var dto = Dto(null, Shared.Contracts.Catalog.LocationLevel.Country, "tr", "tr");
        var ctx = Context(new CreateLocationCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _repo.Received(1).AddLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(Arg.Is<Result<int>>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenChildLocationParentExists_CreatesLocation_RespondsSuccess()
    {
        _repo.GetLocationByIdAsync(1, Arg.Any<CancellationToken>()).Returns(ParentLocation());
        var dto = Dto(1, Shared.Contracts.Catalog.LocationLevel.State, "tr", "34");
        var ctx = Context(new CreateLocationCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _repo.Received(1).AddLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(Arg.Is<Result<int>>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenParentNotFound_RespondsFailure_NotFoundParent()
    {
        _repo.GetLocationByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Location?)null);
        var dto = Dto(99, Shared.Contracts.Catalog.LocationLevel.State, "tr", "34");
        var ctx = Context(new CreateLocationCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _repo.DidNotReceive().AddLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(
            Arg.Is<Result<int>>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_PARENT_LOCATION"));
    }

    [Fact]
    public async Task Consume_WhenInvalidCountryCode_RespondsFailure_RuleViolation()
    {
        // "X" (1-char) → CountryCode ctor DomainException → LOCATION_RULE_VIOLATION
        var dto = Dto(null, Shared.Contracts.Catalog.LocationLevel.Country, "X", "x");
        var ctx = Context(new CreateLocationCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _repo.DidNotReceive().AddLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(
            Arg.Is<Result<int>>(r => r.IsFailure && r.Error!.Code == "LOCATION_RULE_VIOLATION"));
    }

    [Fact]
    public async Task Consume_WhenUnknownLocationLevel_RespondsFailure_RuleViolation()
    {
        // KAYDET-22 defansif _ => throw: out-of-range Contracts enum → DomainException → LOCATION_RULE_VIOLATION
        var dto = Dto(null, (Shared.Contracts.Catalog.LocationLevel)99, "tr", "tr");
        var ctx = Context(new CreateLocationCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _repo.DidNotReceive().AddLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(
            Arg.Is<Result<int>>(r => r.IsFailure && r.Error!.Code == "LOCATION_RULE_VIOLATION"));
    }
}
