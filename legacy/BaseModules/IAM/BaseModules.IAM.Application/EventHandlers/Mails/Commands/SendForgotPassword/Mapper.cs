namespace BaseModules.IAM.Application.EventHandlers.Mails.Queries.SendForgotPassword;

public class Mapper
{
	public ResponseModel MapToResponse(Common.Definitions.Domain.Entities.User user)
	{
		return new ResponseModel
		{
			Users = new List<ResponseModel.ForgotUser>
			{
				new()
				{
					Id = user.Id,
					Email = user.Email,
					DisplayName = $"{user.FirstName} {user.Surname}",
					Token = user.PasswordResetToken
				}
			}
		};
	}
}
