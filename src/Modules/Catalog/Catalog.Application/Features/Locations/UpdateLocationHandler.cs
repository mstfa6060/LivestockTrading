using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Locations;

public sealed class UpdateLocationHandler : IConsumer<UpdateLocationCommand>
{
    private readonly IReferenceDataRepository _repo;

    public UpdateLocationHandler(IReferenceDataRepository repo) => _repo = repo;

    public async Task Consume(ConsumeContext<UpdateLocationCommand> context)
    {
        var ct = context.CancellationToken;
        var cmd = context.Message;

        try
        {
            var location = await _repo.GetLocationByIdAsync(cmd.LocationId, ct);
            if (location is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("NOT_FOUND_LOCATION", $"Location not found: {cmd.LocationId}")));
                return;
            }

            location.Update(cmd.Dto.Name, cmd.Dto.NativeName, cmd.Dto.Population, cmd.Dto.DisplayOrder);
            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(
                new Error("LOCATION_RULE_VIOLATION", ex.Message)));
        }
    }
}
