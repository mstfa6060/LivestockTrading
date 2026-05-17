using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Breeds;

public sealed class UpdateBreedHandler : IConsumer<UpdateBreedCommand>
{
    private readonly IBreedRepository _repo;

    public UpdateBreedHandler(IBreedRepository repo)
    {
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<UpdateBreedCommand> context)
    {
        var ct = context.CancellationToken;
        var dto = context.Message.Dto;

        try
        {
            var breed = await _repo.GetByIdAsync(context.Message.BreedId, ct);
            if (breed is null)
            {
                var error = new Error("NOT_FOUND_BREED", $"Breed not found: {context.Message.BreedId}");
                await context.RespondAsync(Result.Failure(error));
                return;
            }

            // UpdateBreedDto: Name, Description, DisplayOrder (OriginCountryCode stable — DTO'da yok)
            breed.UpdateTranslations(dto.Name, dto.Description);
            breed.UpdateDisplayOrder(dto.DisplayOrder);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            var error = new Error("BREED_RULE_VIOLATION", ex.Message);
            await context.RespondAsync(Result.Failure(error));
        }
    }
}
