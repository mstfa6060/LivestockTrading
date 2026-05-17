using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public sealed class DeactivateCategoryHandler : IConsumer<DeactivateCategoryCommand>
{
    private readonly ICategoryRepository _repo;

    public DeactivateCategoryHandler(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<DeactivateCategoryCommand> context)
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

            category.Deactivate();  // idempotent (Plan-1 teyit: !IsActive return)
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            var error = new Error("CATEGORY_RULE_VIOLATION", ex.Message);
            await context.RespondAsync(Result.Failure(error));
        }
    }
}
