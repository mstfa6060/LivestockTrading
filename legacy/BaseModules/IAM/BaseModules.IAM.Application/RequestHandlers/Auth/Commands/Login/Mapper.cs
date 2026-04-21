namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.Login;

public class Mapper
{
	public ResponseModel MapToResponse(string jwt, DateTime sessionExpirationDate, Common.Definitions.Domain.Entities.User user, string refreshToken)
	{
		var currencyCode = user.PreferredCurrencyCode ?? user.Country?.DefaultCurrencyCode;
		var currencySymbol = user.Country?.DefaultCurrencySymbol;

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