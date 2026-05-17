using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Breeds;

public sealed class DeactivateBreedHandler : IConsumer<DeactivateBreedCommand>
{
    private readonly IBreedRepository _repo;

    public DeactivateBreedHandler(IBreedRepository repo)
    {
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<DeactivateBreedCommand> context)
    {
        var ct = context.CancellationToken;

        try
        {
            var breed = await _repo.GetByIdAsync(context.Message.BreedId, ct);
            if (breed is null)
            {
                var error = new Error("NOT_FOUND_BREED", $"Breed not found: {context.Message.BreedId}");
                await context.RespondAsync(Result.Failure(error));
                return;
            }

            breed.Deactivate();  // idempotent
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            var error = new Error("BREED_RULE_VIOLATION", ex.Message);
            await context.RespondAsync(Result.Failure(error));
        }
    }
}
