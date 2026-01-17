namespace BaseModules.IAM.Application.RequestHandlers.Districts.Queries.ByProvince;

/// <summary>
/// İle Göre İlçeleri Listele
/// Bu endpoint, belirtilen ile ait ilçelerin listesini döner.
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

        var districts = await _dataAccessLayer.GetByProvince(request.ProvinceId, request.Keyword, cancellationToken);
        var response = mapper.MapToResponse(districts);

        return ArfBlocksResults.Success(response);
    }
}
