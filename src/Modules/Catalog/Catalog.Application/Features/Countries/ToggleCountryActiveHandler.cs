using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Countries;

public sealed class ToggleCountryActiveHandler : IConsumer<ToggleCountryActiveCommand>
{
    private readonly IReferenceDataRepository _repo;

    public ToggleCountryActiveHandler(IReferenceDataRepository repo) => _repo = repo;

    public async Task Consume(ConsumeContext<ToggleCountryActiveCommand> context)
    {
        var ct = context.CancellationToken;
        var cmd = context.Message;

        try
        {
            var country = await _repo.GetCountryByIdAsync(cmd.CountryId, ct);
            if (country is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("NOT_FOUND_COUNTRY", $"Country not found: {cmd.CountryId}")));
                return;
            }

            if (cmd.Active) country.Activate();
            else country.Deactivate();

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(
                new Error("COUNTRY_RULE_VIOLATION", ex.Message)));
        }
    }
}
