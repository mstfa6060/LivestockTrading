using FluentValidation;
using Shared.Results;
using MassTransit;

namespace LivestockTrading.Identity.Application.Common.PipelineFilters;

/// <summary>
/// MassTransit pipeline filter — FluentValidation IValidator&lt;T&gt; resolve and
/// validate command before handler invocation. Validator is OPTIONAL: commands
/// without a registered IValidator&lt;T&gt; skip validation and proceed to handler.
/// On validation failure, responds with Result.Failure containing INVALID_REQUEST
/// error code; handler is NOT invoked. Domain-specific validation errors
/// (INVALID_LANGUAGE_CODE, INVALID_CURRENCY_CODE, etc.) are produced by domain
/// logic inside handlers.
/// </summary>
public sealed class ValidationFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly IValidator<T>? _validator;

    public ValidationFilter(IEnumerable<IValidator<T>> validators)
    {
        _validator = validators.FirstOrDefault();
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        if (_validator is not null)
        {
            var result = await _validator.ValidateAsync(context.Message, context.CancellationToken);
            if (!result.IsValid)
            {
                var message = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
                var error = new Error("INVALID_REQUEST", message);
                await context.RespondAsync(Result.Failure(error));
                return;
            }
        }
        await next.Send(context);
    }

    public void Probe(ProbeContext context) => context.CreateFilterScope("validation");
}
