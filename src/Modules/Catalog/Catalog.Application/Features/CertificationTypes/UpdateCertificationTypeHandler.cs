using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;
using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Application.Features.CertificationTypes;

public sealed class UpdateCertificationTypeHandler : IConsumer<UpdateCertificationTypeCommand>
{
    private readonly IReferenceDataRepository _repo;

    public UpdateCertificationTypeHandler(IReferenceDataRepository repo) => _repo = repo;

    public async Task Consume(ConsumeContext<UpdateCertificationTypeCommand> context)
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

            // CertType.DescriptionTranslations field non-nullable; DTO nullable → coalesce (F-S5)
            var description = cmd.Dto.DescriptionTranslations ?? Translations.Empty;
            certType.UpdateTranslations(cmd.Dto.NameTranslations, description);
            certType.UpdateDisplayOrder(cmd.Dto.DisplayOrder);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(
                new Error("CERTIFICATION_TYPE_RULE_VIOLATION", ex.Message)));
        }
    }
}
