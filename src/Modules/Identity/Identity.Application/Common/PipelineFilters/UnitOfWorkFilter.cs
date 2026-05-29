using LivestockTrading.Identity.Application.Abstractions;
using MassTransit;

namespace LivestockTrading.Identity.Application.Common.PipelineFilters;

/// <summary>
/// MassTransit pipeline filter — invokes IUnitOfWork.SaveChangesAsync after
/// handler completes successfully. Aggregate mutations (Add/Remove) performed
/// by handlers are persisted within a single transaction at this point.
/// Failure during handler (exception) skips SaveChanges — transaction is not
/// committed, mutations are discarded.
/// Wave 4 W4.3 Infrastructure wires IUnitOfWork to EF Core DbContext.SaveChangesAsync.
/// </summary>
public sealed class UnitOfWorkFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkFilter(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        await next.Send(context);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);
    }

    public void Probe(ProbeContext context) => context.CreateFilterScope("unit-of-work");
}
