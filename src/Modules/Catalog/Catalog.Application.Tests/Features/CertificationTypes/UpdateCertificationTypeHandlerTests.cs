using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.CertificationTypes;
using LivestockTrading.Catalog.Domain.Entities;
using MassTransit;
using NSubstitute;
using Shared.Contracts.Catalog.Admin;
using Shared.Results;
using Shared.ValueObjects;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.CertificationTypes;

public class UpdateCertificationTypeHandlerTests
{
    private readonly IReferenceDataRepository _repo = Substitute.For<IReferenceDataRepository>();
    private readonly UpdateCertificationTypeHandler _handler;

    public UpdateCertificationTypeHandlerTests() => _handler = new UpdateCertificationTypeHandler(_repo);

    private static ConsumeContext<UpdateCertificationTypeCommand> Context(UpdateCertificationTypeCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<UpdateCertificationTypeCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static CertificationType SampleCertType() =>
        new("health-cert", CertificationTypeTestHelpers.EnglishText("Health"), Translations.Empty, 0);

    [Fact]
    public async Task Consume_WhenCertTypeExists_Updates_RespondsSuccess()
    {
        _repo.GetCertificationTypeByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleCertType());
        var dto = new UpdateCertificationTypeDto(CertificationTypeTestHelpers.EnglishText("New"), null, 5);
        var ctx = Context(new UpdateCertificationTypeCommand(1, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenCertTypeNotFound_RespondsFailure_NotFound()
    {
        _repo.GetCertificationTypeByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((CertificationType?)null);
        var dto = new UpdateCertificationTypeDto(CertificationTypeTestHelpers.EnglishText("New"), null, 0);
        var ctx = Context(new UpdateCertificationTypeCommand(99, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_CERTIFICATION_TYPE"));
    }

    [Fact]
    public async Task Consume_WhenNameMissingEnLocale_RespondsFailure_RuleViolation()
    {
        // UpdateTranslations 'en'-guard (W2.5-A) → DomainException → CERTIFICATION_TYPE_RULE_VIOLATION
        _repo.GetCertificationTypeByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleCertType());
        var dto = new UpdateCertificationTypeDto(Translations.Empty, null, 0);
        var ctx = Context(new UpdateCertificationTypeCommand(1, dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "CERTIFICATION_TYPE_RULE_VIOLATION"));
    }
}
