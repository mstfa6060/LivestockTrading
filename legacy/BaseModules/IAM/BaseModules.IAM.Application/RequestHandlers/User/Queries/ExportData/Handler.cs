namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.ExportData;

/// <summary>
/// Kullanici Verilerini Disa Aktarma (GDPR / KVKK)
/// Kullanicinin tum kisisel verilerini JSON olarak dondurur.
/// </summary>
public class Handler : IRequestHandler
{
    private readonly DataAccess _dataAccess;

    public Handler(ArfBlocksDependencyProvider dp, object dataAccess)
    {
        _dataAccess = (DataAccess)dataAccess;
    }

    public async Task<ArfBlocksRequestResult> Handle(IRequestModel model, EndpointContext ctx, CancellationToken ct)
    {
        var request = (RequestModel)model;
        var mapper = new Mapper();

        var user = await _dataAccess.GetUserById(request.UserId);
        if (user == null)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNotFound));

        var export = mapper.MapToExport(user);

        return ArfBlocksResults.Success(new ResponseModel { Data = export });
    }
}
