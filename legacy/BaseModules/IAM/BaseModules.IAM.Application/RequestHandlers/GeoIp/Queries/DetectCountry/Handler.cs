using BaseModules.IAM.Infrastructure.Services;

namespace BaseModules.IAM.Application.RequestHandlers.GeoIp.Queries.DetectCountry;

/// <summary>
/// IP adresinden ülke tespiti.
/// MaxMind GeoLite2 veritabanı kullanarak istemcinin IP adresinden
/// ülke kodunu tespit eder ve Country bilgisini döner.
/// </summary>
public class Handler : IRequestHandler
{
    private readonly DataAccess _dataAccessLayer;
    private readonly GeoIpService _geoIpService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess)
    {
        _dataAccessLayer = dataAccess;
        _geoIpService = dependencyProvider.GetInstance<GeoIpService>();
        _httpContextAccessor = dependencyProvider.GetInstance<IHttpContextAccessor>();
    }

    public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var mapper = new Mapper();

        // HTTP context'ten client IP al
        var httpContext = _httpContextAccessor.HttpContext;
        var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();

        // X-Forwarded-For header (Nginx/proxy arkasında gerçek IP)
        var forwardedFor = httpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            ipAddress = forwardedFor.Split(',')[0].Trim();
        }

        // GeoIP ile ülke kodu tespit et
        var countryCode = _geoIpService.GetCountryCodeFromIp(ipAddress);

        if (string.IsNullOrEmpty(countryCode))
        {
            return ArfBlocksResults.Success(new ResponseModel
            {
                CountryCode = null,
                CountryId = null,
                CountryName = null
            });
        }

        // Veritabanından ülke bilgisi çek
        var country = await _dataAccessLayer.GetCountryByCode(countryCode, cancellationToken);
        var response = mapper.MapToResponse(country, countryCode);

        return ArfBlocksResults.Success(response);
    }
}
