using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Features.CertificationTypes;
using LivestockTrading.Catalog.Domain.Entities;
using MassTransit;
using NSubstitute;
using Shared.Results;
using Shared.ValueObjects;
using Xunit;

namespace LivestockTrading.Catalog.Application.Tests.Features.CertificationTypes;

public class DeactivateCertificationTypeHandlerTests
{
    private readonly IReferenceDataRepository _repo = Substitute.For<IReferenceDataRepository>();
    private readonly DeactivateCertificationTypeHandler _handler;

    public DeactivateCertificationTypeHandlerTests() => _handler = new DeactivateCertificationTypeHandler(_repo);

    private static ConsumeContext<DeactivateCertificationTypeCommand> Context(DeactivateCertificationTypeCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<DeactivateCertificationTypeCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static CertificationType SampleCertType() =>
        new("health-cert", CertificationTypeTestHelpers.EnglishText("Health"), Translations.Empty, 0);

    [Fact]
    public async Task Consume_WhenCertTypeExists_Deactivates_RespondsSuccess()
    {
        _repo.GetCertificationTypeByIdAsync(1, Arg.Any<CancellationToken>()).Returns(SampleCertType());
        var ctx = Context(new DeactivateCertificationTypeCommand(1, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(Arg.Is<Result>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenCertTypeNotFound_RespondsFailure_NotFound()
    {
        _repo.GetCertificationTypeByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((CertificationType?)null);
        var ctx = Context(new DeactivateCertificationTypeCommand(99, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await ctx.Received(1).RespondAsync(
            Arg.Is<Result>(r => r.IsFailure && r.Error!.Code == "NOT_FOUND_CERTIFICATION_TYPE"));
    }
}
