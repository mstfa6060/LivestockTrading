using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public sealed class RemoveCategoryAttributeHandler : IConsumer<RemoveCategoryAttributeCommand>
{
    private readonly ICategoryRepository _repo;

    public RemoveCategoryAttributeHandler(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<RemoveCategoryAttributeCommand> context)
    {
        var ct = context.CancellationToken;

        try
        {
            var category = await _repo.GetByIdAsync(context.Message.CategoryId, ct);
            if (category is null)
            {
                var error = new Error("NOT_FOUND_CATEGORY", $"Category not found: {context.Message.CategoryId}");
                await context.RespondAsync(Result.Failure(error));
                return;
            }

            // Category.RemoveAttribute(Guid) — not-found'da DomainException (reflex grep L167-174)
            category.RemoveAttribute(context.Message.AttributeId);
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            var error = new Error("CATEGORY_RULE_VIOLATION", ex.Message);
            await context.RespondAsync(Result.Failure(error));
        }
    }
}
