namespace LivestockTrading.Application.RequestHandlers.Students.Commands.Delete;

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
        var requestModel = (RequestModel)payload;

        var entity = await _dataAccessLayer.GetById(requestModel.Id, cancellationToken);
        if (entity == null)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

        // Soft delete
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;

        await _dataAccessLayer.UpdateStudent(entity, cancellationToken);

        var response = mapper.MapToResponse(entity);
        return ArfBlocksResults.Success(response);
    }
}
