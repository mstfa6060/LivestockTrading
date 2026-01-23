namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RefreshToken;

public class Mapper
{
	public ResponseModel Map(string jwt, DateTime expiresAt, string refreshToken, Common.Definitions.Domain.Entities.User user)
	{
		var currencyCode = user.PreferredCurrencyCode ?? user.Country?.DefaultCurrencyCode;
		var currencySymbol = user.Country?.DefaultCurrencySymbol;

		return new ResponseModel()
		{
			Jwt = jwt,
			SessionExpirationDate = expiresAt,
			RefreshToken = refreshToken,
			User = new ResponseModel.UserResponse
			{
				Id = user.Id,
				Username = user.UserName,
				DisplayName = $"{user.FirstName} {user.Surname}",
				Email = user.Email,
				IsPhoneVerified = user.IsPhoneVerified,
				CountryId = user.CountryId,
				CountryCode = user.Country?.Code,
				CountryName = user.Country?.Name,
				Language = user.Language,
				CurrencyCode = currencyCode,
				CurrencySymbol = currencySymbol,
			}
		};
	}
}
