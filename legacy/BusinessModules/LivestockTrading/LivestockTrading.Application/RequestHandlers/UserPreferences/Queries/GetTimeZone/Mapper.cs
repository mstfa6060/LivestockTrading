namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Queries.GetTimeZone;

public class Mapper
{
	public ResponseModel MapToResponse(Guid userId, string timeZone)
	{
		return new ResponseModel
		{
			UserId = userId,
			TimeZone = timeZone ?? string.Empty
		};
	}
}
