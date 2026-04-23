namespace BaseModules.IAM.Application.EventHandlers.Sms.Queries.SendOtpSms;

public class Mapper
{
	public ResponseModel MapToResponse(User user, string message)
	{
		return new ResponseModel
		{
			Users = new List<ResponseModel.OtpSmsUser>
			{
				new()
				{
					Id = user.Id,
					PhoneNumber = user.PhoneNumber,
					DisplayName = $"{user.FirstName} {user.Surname}",
					Message = message
				}
			}
		};
	}
}
