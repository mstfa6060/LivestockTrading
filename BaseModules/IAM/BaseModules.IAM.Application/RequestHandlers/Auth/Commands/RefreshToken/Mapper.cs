namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RefreshToken;

public class Mapper
{
	public ResponseModel Map(string jwt, DateTime expiresAt, string refreshToken)
	{
		return new ResponseModel()
		{
			Jwt = jwt,
			SessionExpirationDate = expiresAt,
			RefreshToken = refreshToken
		};
	}
}
