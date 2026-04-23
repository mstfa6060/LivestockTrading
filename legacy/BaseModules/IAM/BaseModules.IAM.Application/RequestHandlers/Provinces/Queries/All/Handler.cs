using Common.Services.Caching;

namespace BaseModules.IAM.Application.RequestHandlers.Provinces.Queries.All;

/// <summary>
/// Ülkeye göre il/eyalet/bölge listesi
/// CountryId zorunlu, opsiyonel keyword ile filtreleme yapılabilir.
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

        // Keyword yoksa cache kullan (keyword varsa her seferinde DB'ye git)
        List<Province> provinces;
        if (string.IsNullOrWhiteSpace(request.Keyword))
        {
            var cacheKey = CacheKeys.GeoData.Provinces(request.CountryId);
            provinces = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                return await _dataAccessLayer.GetAll(request.CountryId, null, cancellationToken);
            }, TimeSpan.FromHours(24));
        }
        else
        {
            provinces = await _dataAccessLayer.GetAll(request.CountryId, request.Keyword, cancellationToken);
        }

        var response = mapper.MapToResponse(provinces);
        return ArfBlocksResults.Success(response);
    }
}
