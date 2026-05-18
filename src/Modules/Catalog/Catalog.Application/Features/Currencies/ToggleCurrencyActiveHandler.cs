using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Currencies;

public sealed class ToggleCurrencyActiveHandler : IConsumer<ToggleCurrencyActiveCommand>
{
    private readonly IReferenceDataRepository _repo;

    public ToggleCurrencyActiveHandler(IReferenceDataRepository repo) => _repo = repo;

    public async Task Consume(ConsumeContext<ToggleCurrencyActiveCommand> context)
    {
        var ct = context.CancellationToken;
        var cmd = context.Message;

        try
        {
            var currency = await _repo.GetCurrencyByIdAsync(cmd.CurrencyId, ct);
            if (currency is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("NOT_FOUND_CURRENCY", $"Currency not found: {cmd.CurrencyId}")));
                return;
            }

            if (cmd.Active) currency.Activate();
            else currency.Deactivate();

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(
                new Error("CURRENCY_RULE_VIOLATION", ex.Message)));
        }
    }
}
