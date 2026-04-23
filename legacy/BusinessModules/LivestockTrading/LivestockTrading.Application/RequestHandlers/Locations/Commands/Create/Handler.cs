namespace LivestockTrading.Application.RequestHandlers.Locations.Commands.Create;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var entity = mapper.MapToEntity(request);

		// DistrictId varsa ve koordinat belirtilmemişse, ilçenin koordinatlarını kullan
		if (entity.DistrictId.HasValue && (entity.Latitude == null || entity.Latitude == 0) && (entity.Longitude == null || entity.Longitude == 0))
		{
			var district = await _dataAccessLayer.GetDistrictById(entity.DistrictId.Value, cancellationToken);
			if (district != null)
			{
				entity.Latitude = district.Latitude;
				entity.Longitude = district.Longitude;
			}
		}

		await _dataAccessLayer.AddLocation(entity);

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
