namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.UploadEyp;

public class DataAccess : IDataAccess
{
	private readonly DefinitionsDocumentDbContext _documentDbContext;

	public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
	{
		_documentDbContext = dependencyProvider.GetInstance<DefinitionsDocumentDbContext>();
	}
}