using Microsoft.AspNetCore.Authorization;

namespace BaseModules.Notification.Application.RequestHandlers.MobilApplicationVersiyon.Queries.GetVersion;



/// <summary>
/// Uygulama Versiyon Kontrolü
/// Mobil uygulamanın minimum ve güncel versiyon bilgilerini döner.
/// </summary>
public class Handler : IRequestHandler
{
    private readonly DataAccess _dataAccess;

    public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
    {
        _dataAccess = (DataAccess)dataAccess;
    }

    public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var request = (RequestModel)payload;
        var platform = request.Platform?.ToLower() ?? "android";
        var companyId = request.CompanyId;

        var appVersion = await _dataAccess.GetByPlatformAndCompany(platform, companyId);

        if (appVersion == null)
        {
            // Fallback değerler
            return ArfBlocksResults.Success(new ResponseModel
            {
                MinVersion = "1.0.0",
                LatestVersion = "1.0.0",
                ForceUpdate = false,
                UpdateMessage = "",
                StoreUrl = platform == "ios"
                    ? "https://apps.apple.com/app/idXXXXXXXXXX"
                    : "https://play.google.com/store/apps/details?id=com.animalmarket_mobil"
            });
        }

        return ArfBlocksResults.Success(new ResponseModel
        {
            MinVersion = appVersion.MinVersion,
            LatestVersion = appVersion.LatestVersion,
            ForceUpdate = appVersion.ForceUpdate,
            UpdateMessage = appVersion.UpdateMessage,
            StoreUrl = appVersion.StoreUrl
        });
    }
}
