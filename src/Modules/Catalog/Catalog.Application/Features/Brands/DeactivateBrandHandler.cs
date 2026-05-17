using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Brands;

public sealed class DeactivateBrandHandler : IConsumer<DeactivateBrandCommand>
{
    private readonly IBrandRepository _repo;

    public DeactivateBrandHandler(IBrandRepository repo)
    {
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<DeactivateBrandCommand> context)
    {
        var ct = context.CancellationToken;

        try
        {
            var brand = await _repo.GetByIdAsync(context.Message.BrandId, ct);
            if (brand is null)
            {
                var error = new Error("NOT_FOUND_BRAND", $"Brand not found: {context.Message.BrandId}");
                await context.RespondAsync(Result.Failure(error));
                return;
            }

            brand.Deactivate(context.Message.ActorAdminId, reason: null);  // K4: port reason taşımıyor
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            var error = new Error("BRAND_RULE_VIOLATION", ex.Message);
            await context.RespondAsync(Result.Failure(error));
        }
    }
}
