namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.Detail;

public class Mapper
{
	public ResponseModel MapToResponse(Common.Definitions.Domain.Entities.User user)
	{
		return new ResponseModel
		{
			UserId = user.Id,
			UserName = user.UserName,
			Email = user.Email,
			FullName = user.FirstName + " " + user.Surname,

			Language = user.Language,
			IsActive = user.IsActive,
			IsAvailable = user.IsAvailable,
			PhoneNumber = user.PhoneNumber,

			BirthDate = user.BirthDate,
			City = user.City,
			District = user.District,
			IsPhoneVerified = user.IsPhoneVerified
		};
	}
}
