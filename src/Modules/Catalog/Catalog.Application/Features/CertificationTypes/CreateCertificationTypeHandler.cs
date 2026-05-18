using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Entities;
using MassTransit;
using Shared.Domain;
using Shared.Results;
using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Application.Features.CertificationTypes;

public sealed class CreateCertificationTypeHandler : IConsumer<CreateCertificationTypeCommand>
{
    private readonly IReferenceDataRepository _repo;

    public CreateCertificationTypeHandler(IReferenceDataRepository repo) => _repo = repo;

    public async Task Consume(ConsumeContext<CreateCertificationTypeCommand> context)
    {
        var ct = context.CancellationToken;
        var dto = context.Message.Dto;

        try
        {
            // CertType.DescriptionTranslations field non-nullable; DTO nullable → coalesce (F-S5)
            var description = dto.DescriptionTranslations ?? Translations.Empty;

            var certType = new CertificationType(
                dto.Code,
                dto.NameTranslations,
                description,
                dto.DisplayOrder);

            await _repo.AddCertificationTypeAsync(certType, ct);
            await context.RespondAsync<Result<int>>(Result.Success(certType.Id));
        }
        catch (DomainException ex)
        {
            await context.RespondAsync<Result<int>>(Result.Failure<int>(
                new Error("CERTIFICATION_TYPE_RULE_VIOLATION", ex.Message)));
        }
    }
}
