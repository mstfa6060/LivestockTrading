namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Create;

public class Mapper
{
	public Common.Definitions.Domain.Entities.User MapToNewEntity(RequestModel payload, string hashedPassword, string salt)
	{
		return new Common.Definitions.Domain.Entities.User()
		{
			Id = Guid.NewGuid(),
			UserName = payload.UserName,
			FirstName = payload.FirstName,
			Surname = payload.Surname,
			Email = payload.Email,
			PasswordHash = hashedPassword,
			PasswordSalt = salt,
			IsActive = true,
			EmailConfirmed = false,
			ProviderKey = payload.ProviderId, // Google veya Apple ID'si
			AuthProvider = payload.UserSource == UserSources.Google ? "Google" :
						   payload.UserSource == UserSources.Apple ? "Apple" : "Manual",
			UserSource = payload.UserSource,
			Description = payload.Description ?? string.Empty,
			PhoneNumber = payload.PhoneNumber,
			CountryId = payload.CountryId,
			Language = payload.Language,
			PreferredCurrencyCode = payload.PreferredCurrencyCode,
		};
	}


	public ResponseModel MapToResponse(Common.Definitions.Domain.Entities.User user)
	{
		return new ResponseModel()
		{
			Id = user.Id,
			UserName = user.UserName,
			Email = user.Email,
			FirstName = user.FirstName,
			Surname = user.Surname,
			IsActive = user.IsActive,
			CountryId = user.CountryId,
			CountryCode = user.Country?.Code,
			CountryName = user.Country?.Name,
			Language = user.Language,
			CurrencyCode = user.PreferredCurrencyCode,
			CurrencySymbol = null, // TODO: Add CurrencySymbol to Country entity
		};
	}
}
