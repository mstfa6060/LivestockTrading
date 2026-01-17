namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.SendOtp;

public class Mapper
{

	public Common.Definitions.Domain.Entities.User MapToUser(string phoneNumber, Guid companyId, string language)
	{
		return new Common.Definitions.Domain.Entities.User
		{
			Id = Guid.NewGuid(),
			PhoneNumber = phoneNumber,
			CompanyId = companyId,
			Language = language,
			IsActive = true,
			CreatedAt = DateTime.UtcNow,
			UserSource = UserSources.Unknown
		};
	}
}