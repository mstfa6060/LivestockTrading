namespace LivestockTrading.Application.RequestHandlers.Sellers.Queries.Nearby;

public class Handler : IRequestHandler
{
	// Dunya ortalama yaricapi (km)
	private const double EarthRadiusKm = 6371.0;

	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var limit = request.Limit > 0 && request.Limit <= 50 ? request.Limit : 10;

		var candidates = await _dataAccessLayer.GetSellersWithPrimaryLocation(request.CountryCode, cancellationToken);

		// Haversine ile her satici icin mesafe hesapla, sirala ve limit uygula
		var ranked = candidates
			.Select(s => (Seller: s, DistanceKm: Haversine(request.Latitude, request.Longitude, s.Latitude, s.Longitude)))
			.OrderBy(x => x.DistanceKm)
			.Take(limit)
			.ToList();

		var response = mapper.MapToResponse(ranked);
		return ArfBlocksResults.Success(response);
	}

	/// <summary>
	/// Iki cografi nokta arasindaki yay mesafesini kilometre cinsinden dondurur (Haversine formulu).
	/// </summary>
	private static double Haversine(double lat1, double lon1, double lat2, double lon2)
	{
		var dLat = ToRadians(lat2 - lat1);
		var dLon = ToRadians(lon2 - lon1);
		var rLat1 = ToRadians(lat1);
		var rLat2 = ToRadians(lat2);

		var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
				Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(rLat1) * Math.Cos(rLat2);
		var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
		return EarthRadiusKm * c;
	}

	private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
