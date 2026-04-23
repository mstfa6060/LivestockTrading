namespace BaseModules.IAM.Application.RequestHandlers.Countries.Queries.All;

/// <summary>
/// Tüm Ülkeleri Listele
/// Bu endpoint, aktif tüm ülkelerin listesini döner.
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

        var countries = await _dataAccessLayer.GetAll(request.Keyword, cancellationToken);
        var response = mapper.MapToResponse(countries);

        return ArfBlocksResults.Success(response);
    }
}
