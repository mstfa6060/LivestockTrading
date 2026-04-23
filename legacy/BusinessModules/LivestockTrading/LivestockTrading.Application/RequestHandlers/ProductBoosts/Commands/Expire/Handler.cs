namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Commands.Expire;

public class Handler : IRequestHandler
{
    private readonly DataAccess _dataAccessLayer;

    public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
    {
        _dataAccessLayer = (DataAccess)dataAccess;
    }

    public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var expiredBoosts = await _dataAccessLayer.GetExpiredActiveBoosts(cancellationToken);

        if (expiredBoosts.Count == 0)
            return ArfBlocksResults.Success(new ResponseModel { ExpiredCount = 0, ProcessedAt = DateTime.UtcNow });

        var expiredCount = await _dataAccessLayer.ExpireBoosts(expiredBoosts, cancellationToken);

        return ArfBlocksResults.Success(new ResponseModel
        {
            ExpiredCount = expiredCount,
            ProcessedAt = DateTime.UtcNow
        });
    }
}
