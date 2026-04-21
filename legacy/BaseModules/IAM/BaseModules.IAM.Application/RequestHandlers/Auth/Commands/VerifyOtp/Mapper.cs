// namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.SendOtp;

// public class Mapper
// {
// 	public ResponseModel MapToResponse(string accessToken, string refreshToken, DateTime expiresAt)
// 	{
// 		return new ResponseModel
// 		{
// 			AccessToken = accessToken,
// 			RefreshToken = refreshToken,
// 			ExpiresAt = expiresAt
// 		};
// 	}

// 	public User MapToUser(string phoneNumber, Guid companyId, string language)
// 	{
// 		return new User
// 		{
// 			Id = Guid.NewGuid(),
// 			PhoneNumber = phoneNumber,
// 			CompanyId = companyId,
// 			Language = language,
// 			IsActive = true,
// 			CreatedAt = DateTime.UtcNow,
// 			UserType = UserType.WorkerAndEmployer,
// 			UserSource = UserSources.Unknown
// 		};
// 	}
// }