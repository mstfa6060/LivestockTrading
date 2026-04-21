namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.ForgotPassword;

public class Mapper
{
	public ResponseModel MapToResponse(Common.Definitions.Domain.Entities.User user)
	{
		return new ResponseModel
		{
			UserId = user.Id,
			Email = user.Email,
			TokenExpiry = user.PasswordResetTokenExpiry
		};
	}
}
