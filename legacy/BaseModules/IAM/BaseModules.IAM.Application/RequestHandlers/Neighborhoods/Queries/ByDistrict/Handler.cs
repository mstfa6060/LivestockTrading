namespace BaseModules.IAM.Application.RequestHandlers.Neighborhoods.Queries.ByDistrict;

/// <summary>
/// İlçeye Göre Mahalleleri Listele
/// Bu endpoint, belirtilen ilçeye ait mahallelerin listesini döner.
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

        var neighborhoods = await _dataAccessLayer.GetByDistrict(request.DistrictId, request.Keyword, cancellationToken);
        var response = mapper.MapToResponse(neighborhoods);

        return ArfBlocksResults.Success(response);
    }
}
