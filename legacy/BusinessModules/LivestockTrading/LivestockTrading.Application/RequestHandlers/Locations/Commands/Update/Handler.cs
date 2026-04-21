namespace LivestockTrading.Application.RequestHandlers.Locations.Commands.Update;

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

		var location = await _dataAccessLayer.GetLocationById(request.Id);

		if (location == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, location);

		// DistrictId varsa ve koordinat belirtilmemişse, ilçenin koordinatlarını kullan
		if (location.DistrictId.HasValue && (location.Latitude == null || location.Latitude == 0) && (location.Longitude == null || location.Longitude == 0))
		{
			var district = await _dataAccessLayer.GetDistrictById(location.DistrictId.Value, cancellationToken);
			if (district != null)
			{
				location.Latitude = district.Latitude;
				location.Longitude = district.Longitude;
			}
		}

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(location);
		return ArfBlocksResults.Success(response);
	}
}
