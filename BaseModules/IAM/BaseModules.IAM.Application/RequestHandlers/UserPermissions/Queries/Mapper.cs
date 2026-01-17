namespace BaseModules.IAM.Application.RequestHandlers.UserPermissions.Queries.My;

public class Mapper
{
	public ResponseModel MapToResponse(
		List<string> permissions,
		List<ResourceDto> resources)
	{
		return new ResponseModel
		{
			Permissions = permissions,
			Resources = resources.Select(r => new ResourceModel
			{
				Namespace = r.Namespace,
				Name = r.Name,
				Title = r.Title
			}).ToList()
		};
	}
}