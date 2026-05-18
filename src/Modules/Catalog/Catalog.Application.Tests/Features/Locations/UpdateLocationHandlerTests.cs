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

public class UpdateLocationHandlerTests
{
    private readonly IReferenceDataRepository _repo = Substitute.For<IReferenceDataRepository>();
    private readonly UpdateLocationHandler _handler;

    public UpdateLocationHandlerTests() => _handler = new UpdateLocationHandler(_repo);

    private static ConsumeContext<UpdateLocationCommand> Context(UpdateLocationCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<UpdateLocationCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static Location SampleLocation() =>
        new(null, LocationLevel.Country, new CountryCode("tr"), "tr", "tr", "tr",
            LocationTestHelpers.EnglishText("Turkey"), null, null, 0, 0);

    // NOT: Location.Update guard'sız (W2.4-A) → DomainException path YOK → LOCATION_RULE_VIOLATION
    // testi yazılamaz (handler catch'i emsal-yapısal defansif, ulaşılamaz). UpdateBreed'in
    // 'en'-guard'lı UpdateTranslations'ı vardı; Location.Update yok. 2 test (W2.2 emsal 3 yerine).

    [Fact]
    public async Task Consume_WhenLocationExists_Updates_RespondsSuccess()
    {
        _repo.GetLocationByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleLocation());
        var dto = new UpdateLocationDto(LocationTestHelpers.EnglishText("New"), null, 100, 5);
        var ctx = Context(new UpdateLocationCommand(1, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenLocationNotFound_RespondsFailure_NotFound()
    {
        _repo.GetLocationByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Location?)null);
        var dto = new UpdateLocationDto(LocationTestHelpers.EnglishText("New"), null, 0, 0);
        var ctx = Context(new UpdateLocationCommand(99, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_LOCATION"));
    }
}
