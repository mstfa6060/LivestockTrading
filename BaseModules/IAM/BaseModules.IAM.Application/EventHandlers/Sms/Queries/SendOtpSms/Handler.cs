using BaseModules.IAM.Infrastructure.RelationalDB;
using Common.Definitions.Domain.Entities;

namespace BaseModules.IAM.Application.EventHandlers.Sms.Queries.SendOtpSms;

[InternalHandler]
[EventHandler]
public class Handler : IRequestHandler
{
	private readonly IamDbContext _dbContext;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dbContext = dependencyProvider.GetInstance<IamDbContext>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var user = await _dbContext.AppUsers.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);


		var response = new ResponseModel
		{
			Users = new List<ResponseModel.OtpSmsUser>
			{
				new()
				{
					Id = user.Id,
					PhoneNumber = user.PhoneNumber,
					DisplayName = $"{user.FirstName} {user.Surname}",
					Message = request.Message
				}
			}
		};

		return ArfBlocksResults.Success(response);
	}
}
