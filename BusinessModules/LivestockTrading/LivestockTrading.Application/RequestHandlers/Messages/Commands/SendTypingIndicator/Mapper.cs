namespace LivestockTrading.Application.RequestHandlers.Messages.Commands.SendTypingIndicator;

public class Mapper
{
	public ResponseModel MapToResponse(bool success)
	{
		return new ResponseModel
		{
			Success = success
		};
	}
}
