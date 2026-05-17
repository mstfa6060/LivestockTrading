using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Brands;

public sealed class ApproveBrandHandler : IConsumer<ApproveBrandCommand>
{
    private readonly IBrandRepository _repo;

    public ApproveBrandHandler(IBrandRepository repo)
    {
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<ApproveBrandCommand> context)
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

            brand.Approve(context.Message.ActorAdminId);  // Suggested → Approved (Status!=Suggested → throw)
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            var error = new Error("BRAND_RULE_VIOLATION", ex.Message);
            await context.RespondAsync(Result.Failure(error));
        }
    }
}
