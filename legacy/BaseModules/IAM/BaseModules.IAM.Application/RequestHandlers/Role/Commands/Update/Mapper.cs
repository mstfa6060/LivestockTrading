namespace BaseModules.IAM.Application.RequestHandlers.Roles.Commands.Update;

public class Mapper
{
	public void MapToExistingEntity(RequestModel payload, Role role)
	{
		role.Name = payload.Name;
	}

	public ResponseModel MapToResponse(Role role)
	{
		return new ResponseModel()
		{
			Id = role.Id
		};
	}
}
