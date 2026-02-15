namespace BaseModules.IAM.Application.RequestHandlers.Geo.Queries.Detect;

/// <summary>
/// IP Adresine Göre Ülke Tespiti
/// Bu endpoint, kullanıcının IP adresinden ülke, para birimi, dil ve saat dilimi bilgilerini tespit eder.
/// Public endpoint - kimlik doğrulama gerektirmez.
/// </summary>
public class Handler : IRequestHandler
{
    private readonly IGeoLocationService _geoLocationService;
    private readonly CurrentUserService _currentUserService;

    public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
    {
        _geoLocationService = dependencyProvider.GetInstance<IGeoLocationService>();
        _currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
    }

    public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var ipAddress = _currentUserService.GetCurrentUserIp()?.ToString();

        var geoResult = _geoLocationService.DetectCountry(ipAddress);

        var response = new ResponseModel
        {
            CountryCode = geoResult.CountryCode,
            CountryName = geoResult.CountryName,
            Currency = geoResult.Currency,
            Language = geoResult.Language,
            Timezone = geoResult.Timezone
        };

        return ArfBlocksResults.Success(response);
    }
}
