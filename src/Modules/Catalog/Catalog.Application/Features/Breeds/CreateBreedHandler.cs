using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Breeds;

public sealed class CreateBreedHandler : IConsumer<CreateBreedCommand>
{
    private readonly IBreedRepository _breedRepo;
    private readonly ICategoryRepository _categoryRepo;

    public CreateBreedHandler(IBreedRepository breedRepo, ICategoryRepository categoryRepo)
    {
        _breedRepo = breedRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task Consume(ConsumeContext<CreateBreedCommand> context)
    {
        var dto = context.Message.Dto;
        var ct = context.CancellationToken;

        try
        {
            var category = await _categoryRepo.GetByCodeAsync(dto.CategoryCode, ct);
            if (category is null)
            {
                var notFoundError = new Error(
                    "NOT_FOUND_PARENT_CATEGORY",
                    $"Parent category not found: {dto.CategoryCode}");
                await context.RespondAsync<Result<int>>(Result.Failure<int>(notFoundError));
                return;
            }

            var breed = Breed.Create(
                dto.Code,
                category,
                dto.Name,
                dto.Description,
                dto.OriginCountryCode,
                dto.DisplayOrder);

            await _breedRepo.AddAsync(breed, ct);
            await context.RespondAsync<Result<int>>(Result.Success(breed.Id));
        }
        catch (DomainException ex)
        {
            var error = new Error("BREED_RULE_VIOLATION", ex.Message);
            await context.RespondAsync<Result<int>>(Result.Failure<int>(error));
        }
    }
}
