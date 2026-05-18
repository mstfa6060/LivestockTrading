using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Application.Common.Mappers;
using MassTransit;
using Shared.Domain;
using Shared.Results;

namespace LivestockTrading.Catalog.Application.Features.BorderRules;

public sealed class UpdateBorderRuleHandler : IConsumer<UpdateBorderRuleCommand>
{
    private readonly IBorderRuleRepository _repo;

    public UpdateBorderRuleHandler(IBorderRuleRepository repo) => _repo = repo;

    public async Task Consume(ConsumeContext<UpdateBorderRuleCommand> context)
    {
        var ct = context.CancellationToken;
        var cmd = context.Message;

        try
        {
            var borderRule = await _repo.GetByIdAsync(cmd.BorderRuleId, ct);
            if (borderRule is null)
            {
                await context.RespondAsync(Result.Failure(
                    new Error("NOT_FOUND_BORDER_RULE", $"BorderRule not found: {cmd.BorderRuleId}")));
                return;
            }

            var kind = BorderRuleKindMapper.ToDomain(cmd.Dto.Kind);
            borderRule.Update(
                kind,
                cmd.Dto.RestrictionsJson,
                cmd.Dto.Notes,
                cmd.Dto.EffectiveFrom,
                cmd.Dto.EffectiveUntil,
                cmd.ActorAdminId);

            await context.RespondAsync(Result.Success());
        }
        catch (DomainException ex)
        {
            await context.RespondAsync(Result.Failure(
                new Error("BORDER_RULE_VIOLATION", ex.Message)));
        }
    }
}
