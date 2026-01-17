namespace BaseModules.IAM.Application.RequestHandlers.Roles.Commands.Delete;

public class Mapper
{
	public ResponseModel MapToResponse(Role entity)
	{
		return new ResponseModel
		{
			Id = entity.Id,
			IsDeleted = entity.IsDeleted
		};
	}
}
