using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public sealed class UpdateCategoryHandler : IConsumer<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _repo;

    public UpdateCategoryHandler(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<UpdateCategoryCommand> context)
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

            // UpdateCategoryDto 4 alan birlikte: Name, Description, DisplayOrder, IconKey
            category.UpdateTranslations(dto.Name, dto.Description);
            category.UpdateDisplayOrder(dto.DisplayOrder);
            category.UpdateIconKey(dto.IconKey);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            var error = new Error("CATEGORY_RULE_VIOLATION", ex.Message);
            await context.RespondAsync(Result.Failure(error));
        }
    }
}
