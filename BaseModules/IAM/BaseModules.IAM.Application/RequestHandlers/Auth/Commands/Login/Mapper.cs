namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.Login;

public class Mapper
{
	public ResponseModel MapToResponse(string jwt, DateTime sessionExpirationDate, Common.Definitions.Domain.Entities.User user, string refreshToken)
	{
		return new ResponseModel
		{
			Jwt = jwt,
			SessionExpirationDate = sessionExpirationDate,
			RefreshToken = refreshToken,
			User = new ResponseModel.UserResponse
			{
				Id = user.Id,
				Username = user.UserName,
				DisplayName = $"{user.FirstName} {user.Surname}",
				Email = user.Email,
				IsPhoneVerified = user.IsPhoneVerified
			}
		};
	}
}