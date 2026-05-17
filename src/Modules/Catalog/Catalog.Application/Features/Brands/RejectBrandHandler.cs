using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Brands;

public sealed class RejectBrandHandler : IConsumer<RejectBrandCommand>
{
    private readonly IBrandRepository _repo;

    public RejectBrandHandler(IBrandRepository repo)
    {
        _repo = repo;
    }

    public async Task Consume(ConsumeContext<RejectBrandCommand> context)
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

            brand.Reject(context.Message.ActorAdminId, context.Message.Reason);  // Suggested → Rejected
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            var error = new Error("BRAND_RULE_VIOLATION", ex.Message);
            await context.RespondAsync(Result.Failure(error));
        }
    }
}
