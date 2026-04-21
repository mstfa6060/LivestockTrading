namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Common.Definitions.Domain.Entities.User> users, Dictionary<Guid, List<string>> userRoles)
	{
		return users.Select(user => new ResponseModel
		{
			UserId = user.Id,
			UserName = user.UserName,
			Email = user.Email,
			FullName = user.FirstName + " " + user.Surname,
			IsActive = user.IsActive,
			IsAvailable = user.IsAvailable,
			PhoneNumber = user.PhoneNumber,
			BucketId = user.BucketId,
			Roles = userRoles.ContainsKey(user.Id) ? userRoles[user.Id] : new List<string>()
		}).ToList();
	}
}
