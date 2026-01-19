namespace LivestockTrading.Application.RequestHandlers.Students.Queries.Detail;

public class Handler : IRequestHandler
{
    private readonly DataAccess _dataAccessLayer;

    public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
    {
        _dataAccessLayer = (DataAccess)dataAccess;
    }

    public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var mapper = new Mapper();
        var req = (RequestModel)payload;

        var student = await _dataAccessLayer.GetById(req.Id, cancellationToken);
        if (student == null)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

        var response = mapper.MapToResponse(student);

        return ArfBlocksResults.Success(response);
    }
}
