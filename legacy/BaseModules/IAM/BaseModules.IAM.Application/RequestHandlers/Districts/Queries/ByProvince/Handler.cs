using Common.Services.Caching;

namespace BaseModules.IAM.Application.RequestHandlers.Districts.Queries.ByProvince;

/// <summary>
/// İl/bölgeye göre ilçe/şehir listesi
/// ProvinceId zorunlu, opsiyonel keyword ile filtreleme yapılabilir.
/// Sonuçlar 24 saat Redis'te cache'lenir.
/// </summary>
public class Handler : IRequestHandler
{
    private readonly DataAccess _dataAccessLayer;
    private readonly ICacheService _cacheService;

    public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess)
    {
        _dataAccessLayer = dataAccess;
        _cacheService = dependencyProvider.GetInstance<ICacheService>();
    }

    public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var request = (RequestModel)payload;
        var mapper = new Mapper();

        List<District> districts;
        if (string.IsNullOrWhiteSpace(request.Keyword))
        {
            var cacheKey = CacheKeys.GeoData.Districts(request.ProvinceId);
            districts = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                return await _dataAccessLayer.GetByProvince(request.ProvinceId, null, cancellationToken);
            }, TimeSpan.FromHours(24));
        }
        else
        {
            districts = await _dataAccessLayer.GetByProvince(request.ProvinceId, request.Keyword, cancellationToken);
        }

        var response = mapper.MapToResponse(districts);
        return ArfBlocksResults.Success(response);
    }
}
