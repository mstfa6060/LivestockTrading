namespace BaseModules.IAM.Application.RequestHandlers.Provinces.Queries.All;

/// <summary>
/// Tüm İlleri Listele
/// Bu endpoint, Türkiye'deki tüm illerin listesini döner.
/// Opsiyonel keyword ile filtreleme yapılabilir.
/// </summary>
public class Handler : IRequestHandler
{
    private readonly DataAccess _dataAccessLayer;

    public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess)
    {
        _dataAccessLayer = dataAccess;
    }

    public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var request = (RequestModel)payload;
        var mapper = new Mapper();

        var provinces = await _dataAccessLayer.GetAll(request.Keyword, cancellationToken);
        var response = mapper.MapToResponse(provinces);

        return ArfBlocksResults.Success(response);
    }
}
