namespace BaseModules.IAM.Application.RequestHandlers.Countries.Queries.GetByCode;

/// <summary>
/// Ülke Kodu ile Ülke Detayını Getir
/// ISO 3166-1 alpha-2 koduna göre ülke bilgilerini döner.
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

        var country = await _dataAccessLayer.GetByCode(request.Code, cancellationToken);
        if (country == null)
            throw new ArfBlocksValidationException("Country not found for the given code.");

        var response = mapper.MapToResponse(country);
        return ArfBlocksResults.Success(response);
    }
}
