namespace BaseModules.IAM.Application.RequestHandlers.Roles.Queries.All;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<Role> roles)
	{
		var mappedRoles = new List<ResponseModel>();

		foreach (var role in roles)
		{
			mappedRoles.Add(new ResponseModel()
			{
				Id = role.Id,
				Name = role.Name,
				IsDeleted = role.IsDeleted
			});
		}

		return mappedRoles;
	}
}
