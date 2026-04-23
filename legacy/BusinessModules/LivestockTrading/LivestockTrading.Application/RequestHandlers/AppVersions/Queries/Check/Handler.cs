using LivestockTrading.Application.Extensions;

namespace LivestockTrading.Application.RequestHandlers.AppVersions.Queries.Check;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var req = (RequestModel)payload;

		var cfg = await _dataAccessLayer.GetActiveByPlatform(req.Platform, cancellationToken);

		// Yapilandirma yoksa veya pasifse: guncelleme yok
		if (cfg == null || !cfg.IsActive)
		{
			return ArfBlocksResults.Success(new ResponseModel
			{
				MinSupportedVersion = cfg?.MinSupportedVersion,
				LatestVersion = cfg?.LatestVersion,
				StoreUrl = cfg?.StoreUrl,
				UpdateMessage = cfg?.UpdateMessage,
				UpdateType = 0
			});
		}

		var updateType = 0;
		if (SemanticVersionComparer.Compare(req.CurrentVersion, cfg.MinSupportedVersion) < 0)
		{
			updateType = 2; // Force
		}
		else if (SemanticVersionComparer.Compare(req.CurrentVersion, cfg.LatestVersion) < 0)
		{
			updateType = 1; // Soft
		}

		return ArfBlocksResults.Success(new ResponseModel
		{
			MinSupportedVersion = cfg.MinSupportedVersion,
			LatestVersion = cfg.LatestVersion,
			StoreUrl = cfg.StoreUrl,
			UpdateMessage = cfg.UpdateMessage,
			UpdateType = updateType
		});
	}
}
