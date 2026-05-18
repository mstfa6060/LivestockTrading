using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Common.Mappers;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using Shared.Domain;
using Shared.Results;
using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Application.Features.BorderRules;

public sealed class CreateBorderRuleHandler : IConsumer<CreateBorderRuleCommand>
{
    private readonly IBorderRuleRepository _borderRuleRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IBreedRepository _breedRepo;

    public CreateBorderRuleHandler(
        IBorderRuleRepository borderRuleRepo,
        ICategoryRepository categoryRepo,
        IBreedRepository breedRepo)
    {
        _borderRuleRepo = borderRuleRepo;
        _categoryRepo = categoryRepo;
        _breedRepo = breedRepo;
    }

    public async Task Consume(ConsumeContext<CreateBorderRuleCommand> context)
    {
        var ct = context.CancellationToken;
        var dto = context.Message.Dto;

        try
        {
            // CountryCode VO ctor (Parse/From YOK); geçersiz → DomainException → BORDER_RULE_VIOLATION
            var fromCountry = new CountryCode(dto.FromCountryCode);
            var toCountry = new CountryCode(dto.ToCountryCode);

            // CategoryCode/BreedCode Code-only → server-side int? resolve (null = genel kural)
            int? categoryId = null;
            if (dto.CategoryCode is not null)
            {
                var category = await _categoryRepo.GetByCodeAsync(dto.CategoryCode, ct);
                if (category is null)
                {
                    await context.RespondAsync<Result<Guid>>(Result.Failure<Guid>(
                        new Error("NOT_FOUND_CATEGORY_FOR_BORDER_RULE", $"Category not found: {dto.CategoryCode}")));
                    return;
                }
                categoryId = category.Id;
            }

            int? breedId = null;
            if (dto.BreedCode is not null)
            {
                var breed = await _breedRepo.GetByCodeAsync(dto.BreedCode, ct);
                if (breed is null)
                {
                    await context.RespondAsync<Result<Guid>>(Result.Failure<Guid>(
                        new Error("NOT_FOUND_BREED_FOR_BORDER_RULE", $"Breed not found: {dto.BreedCode}")));
                    return;
                }
                breedId = breed.Id;
            }

            var kind = BorderRuleKindMapper.ToDomain(dto.Kind);

            var borderRule = BorderRule.Create(
                fromCountry,
                toCountry,
                kind,
                dto.RestrictionsJson,
                dto.Notes,
                context.Message.ActorAdminId,
                categoryId,
                breedId,
                dto.EffectiveFrom,
                dto.EffectiveUntil);

            await _borderRuleRepo.AddAsync(borderRule, ct);
            await context.RespondAsync<Result<Guid>>(Result.Success(borderRule.Id));
        }
        catch (DomainException ex)
        {
            await context.RespondAsync<Result<Guid>>(Result.Failure<Guid>(
                new Error("BORDER_RULE_VIOLATION", ex.Message)));
        }
    }
}
