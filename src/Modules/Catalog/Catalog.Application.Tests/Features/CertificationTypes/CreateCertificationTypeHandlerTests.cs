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

public class CreateCertificationTypeHandlerTests
{
    private readonly IReferenceDataRepository _repo = Substitute.For<IReferenceDataRepository>();
    private readonly CreateCertificationTypeHandler _handler;

    public CreateCertificationTypeHandlerTests() => _handler = new CreateCertificationTypeHandler(_repo);

    private static ConsumeContext<CreateCertificationTypeCommand> Context(CreateCertificationTypeCommand cmd)
    {
        var ctx = Substitute.For<ConsumeContext<CreateCertificationTypeCommand>>();
        ctx.Message.Returns(cmd);
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    private static CreateCertificationTypeDto Dto(string code, Translations name, Translations? description) =>
        new(code, name, description, 0);

    [Fact]
    public async Task Consume_WhenDescriptionProvided_CreatesCertType_RespondsSuccess()
    {
        var dto = Dto("health-cert", CertificationTypeTestHelpers.EnglishText("Health"),
            CertificationTypeTestHelpers.EnglishText("Desc"));
        var ctx = Context(new CreateCertificationTypeCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _repo.Received(1).AddCertificationTypeAsync(Arg.Any<CertificationType>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(Arg.Is<Result<int>>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenDescriptionNull_CoalescesToEmpty_RespondsSuccess()
    {
        var dto = Dto("health-cert", CertificationTypeTestHelpers.EnglishText("Health"), null);
        var ctx = Context(new CreateCertificationTypeCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _repo.Received(1).AddCertificationTypeAsync(Arg.Any<CertificationType>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(Arg.Is<Result<int>>(r => r.IsSuccess));
    }

    [Fact]
    public async Task Consume_WhenCodeEmpty_RespondsFailure_RuleViolation()
    {
        // code "" → CertificationType ctor DomainException → CERTIFICATION_TYPE_RULE_VIOLATION
        var dto = Dto("", CertificationTypeTestHelpers.EnglishText("Health"), null);
        var ctx = Context(new CreateCertificationTypeCommand(dto, Guid.NewGuid()));

        await _handler.Consume(ctx);

        await _repo.DidNotReceive().AddCertificationTypeAsync(Arg.Any<CertificationType>(), Arg.Any<CancellationToken>());
        await ctx.Received(1).RespondAsync<Result<int>>(
            Arg.Is<Result<int>>(r => r.IsFailure && r.Error!.Code == "CERTIFICATION_TYPE_RULE_VIOLATION"));
    }
}
