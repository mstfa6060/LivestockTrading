// namespace LivestockTrading.Application.RequestHandlers.Students.Commands.Create;

// public class Handler : IRequestHandler
// {
//     private readonly DataAccess _dataAccessLayer;

//     public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
//     {
//         _dataAccessLayer = (DataAccess)dataAccess;
//     }

//     public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
//     {
//         var mapper = new Mapper();
//         var requestModel = (RequestModel)payload;

//         var entity = mapper.MapToNewEntity(requestModel);

//         await _dataAccessLayer.AddStudent(entity, cancellationToken);

//         var response = mapper.MapToResponse(entity);
//         return ArfBlocksResults.Success(response);
//     }
// }
