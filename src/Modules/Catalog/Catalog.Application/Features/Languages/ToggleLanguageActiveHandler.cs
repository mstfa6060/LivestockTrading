using LivestockTrading.Catalog.Application.Abstractions;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.Languages;

public sealed class ToggleLanguageActiveHandler : IConsumer<ToggleLanguageActiveCommand>
{
    private readonly IReferenceDataRepository _repo;

    public ToggleLanguageActiveHandler(IReferenceDataRepository repo) => _repo = repo;

    public async Task Consume(ConsumeContext<ToggleLanguageActiveCommand> context)
    {
        var ct = context.CancellationToken;
        var cmd = context.Message;

        try
        {
            var language = await _repo.GetLanguageByIdAsync(cmd.LanguageId, ct);
            if (language is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("NOT_FOUND_LANGUAGE", $"Language not found: {cmd.LanguageId}")));
                return;
            }

            if (cmd.Active) language.Activate();
            else language.Deactivate();

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(
                new Error("LANGUAGE_RULE_VIOLATION", ex.Message)));
        }
    }
}
