using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.CertificationTypes;

public sealed class DeactivateCertificationTypeHandler : IConsumer<DeactivateCertificationTypeCommand>
{
    private readonly IReferenceDataRepository _repo;

    public DeactivateCertificationTypeHandler(IReferenceDataRepository repo) => _repo = repo;

    public async Task Consume(ConsumeContext<DeactivateCertificationTypeCommand> context)
    {
        var ct = context.CancellationToken;
        var cmd = context.Message;

        try
        {
            var certType = await _repo.GetCertificationTypeByIdAsync(cmd.CertificationTypeId, ct);
            if (certType is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("NOT_FOUND_CERTIFICATION_TYPE", $"CertificationType not found: {cmd.CertificationTypeId}")));
                return;
            }

            certType.Deactivate();
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(
                new Error("CERTIFICATION_TYPE_RULE_VIOLATION", ex.Message)));
        }
    }
}
