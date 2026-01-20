
namespace BaseModules.IAM.Application.RequestHandlers.Roles.Commands.Create;

public class Mapper
{
	public Role MapToNewEntity(RequestModel payload)
	{
		return new Role()
		{
			Id = payload.Id,
			Name = payload.Name,
		};
	}

	public ResponseModel MapToResponse(Role role)
	{
		return new ResponseModel()
		{
			Id = role.Id
		};
	}
}