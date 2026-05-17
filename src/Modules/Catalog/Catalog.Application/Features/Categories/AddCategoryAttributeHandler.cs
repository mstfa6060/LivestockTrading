using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public sealed class AddCategoryAttributeHandler : IConsumer<AddCategoryAttributeCommand>
{
    private readonly ICategoryRepository _repo;

    public AddCategoryAttributeHandler(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<AddCategoryAttributeCommand> context)
    {
        var ct = context.CancellationToken;
        var dto = context.Message.Dto;

        try
        {
            var category = await _repo.GetByIdAsync(context.Message.CategoryId, ct);
            if (category is null)
            {
                var error = new Error("NOT_FOUND_CATEGORY", $"Category not found: {context.Message.CategoryId}");
                await context.RespondAsync(Result.Failure(error));
                return;
            }

            // Contracts AttributeValueType → Domain AttributeValueType explicit map (KAYDET-22;
            // çift-enum bilinçli Domain duplikasyonu, switch exhaustiveness compiler-enforced)
            var domainValueType = dto.ValueType switch
            {
                Shared.Contracts.Catalog.AttributeValueType.Text => LivestockTrading.Catalog.Domain.Entities.AttributeValueType.Text,
                Shared.Contracts.Catalog.AttributeValueType.Number => LivestockTrading.Catalog.Domain.Entities.AttributeValueType.Number,
                Shared.Contracts.Catalog.AttributeValueType.Boolean => LivestockTrading.Catalog.Domain.Entities.AttributeValueType.Boolean,
                Shared.Contracts.Catalog.AttributeValueType.Enum => LivestockTrading.Catalog.Domain.Entities.AttributeValueType.Enum,
                Shared.Contracts.Catalog.AttributeValueType.Date => LivestockTrading.Catalog.Domain.Entities.AttributeValueType.Date,
                Shared.Contracts.Catalog.AttributeValueType.File => LivestockTrading.Catalog.Domain.Entities.AttributeValueType.File,
                _ => throw new InvalidOperationException($"Unknown AttributeValueType: {dto.ValueType}")
            };

            // Category.AddAttribute 9 param — AttributeDto field sırasıyla 1:1 (reflex grep teyit L140-149)
            category.AddAttribute(
                dto.Key,
                domainValueType,
                dto.Required,
                dto.Filterable,
                dto.Unit,
                dto.OptionsJson,
                dto.Label,
                dto.HelpText,
                dto.DisplayOrder);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            var error = new Error("CATEGORY_RULE_VIOLATION", ex.Message);
            await context.RespondAsync(Result.Failure(error));
        }
    }
}
