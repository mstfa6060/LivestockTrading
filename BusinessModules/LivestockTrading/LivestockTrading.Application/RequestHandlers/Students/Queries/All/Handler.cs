namespace LivestockTrading.Application.RequestHandlers.Students.Queries.All;

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

        var (students, page) = await _dataAccessLayer.All(
            req.Sorting,
            req.Filters,
            req.PageRequest,
            cancellationToken);

        var response = mapper.MapToResponse(students);

        return ArfBlocksResults.Success(response, page);
    }
}
