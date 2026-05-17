using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Categories;

public sealed class CreateCategoryHandler : IConsumer<CreateCategoryCommand>
{
    private readonly ICategoryRepository _repo;

    public CreateCategoryHandler(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<CreateCategoryCommand> context)
    {
        var dto = context.Message.Dto;
        var ct = context.CancellationToken;

        try
        {
            Category category;
            if (dto.ParentCode is null)
            {
                category = Category.CreateTopLevel(
                    dto.Code, dto.Name, dto.Description, dto.IconKey, dto.DisplayOrder);
            }
            else
            {
                var parent = await _repo.GetByCodeAsync(dto.ParentCode, ct);
                if (parent is null)
                {
                    var notFoundError = new Error(
                        "NOT_FOUND_PARENT_CATEGORY",
                        $"Parent category not found: {dto.ParentCode}");
                    await context.RespondAsync<Result<int>>(Result.Failure<int>(notFoundError));
                    return;
                }

                category = Category.CreateSubcategory(
                    dto.Code, parent, dto.Name, dto.Description, dto.IconKey, dto.DisplayOrder);
            }

            await _repo.AddAsync(category, ct);
            await context.RespondAsync<Result<int>>(Result.Success(category.Id));
        }
        catch (DomainException ex)
        {
            var error = new Error("CATEGORY_RULE_VIOLATION", ex.Message);
            await context.RespondAsync<Result<int>>(Result.Failure<int>(error));
        }
    }
}
