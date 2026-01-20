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
			IsActive = user.IsActive
		};
	}
}
