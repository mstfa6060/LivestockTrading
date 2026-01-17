namespace Common.Services.FileOperations.FileStorage;

public class DefinitionModuleStorage : IModuleStorage
{
	public DefinitionModuleStorage()
	{ }

	public string GetDirectoryRelativePathForUploadFile(string basePath, string moduleName, Guid tenantId, string folderName, Guid? entityId, string versionName)
	{
		var newPathName = Path.Combine(basePath, "modules", moduleName, folderName, entityId.ToString());

		if (!Directory.Exists(newPathName))
			Directory.CreateDirectory(newPathName);

		newPathName = newPathName.Replace(basePath, "");
		newPathName = newPathName.StartsWith("/") ? newPathName.Substring(1, newPathName.Length - 1) : newPathName;

		return newPathName;
	}

	public string GetNormalizedFileName(string originalFileName, string extention)
	{
		originalFileName = string.Concat(Guid.NewGuid().ToString().Replace("-", ""), "_", originalFileName);
		//TODO: %, [,], *, <,>, linux unexcepted + eyp unexcepted chars replace with _
		return originalFileName.Replace(" ", "_").Replace(".", "_").Replace("(", "").Replace(")", "").Replace("%", "_").Replace("[", "_").Replace("]", "_").Replace("*", "_").Replace("<", "_").Replace(">", "_").Replace("$", "_").Replace("@", "_").Replace("^", "_").Replace("₺", "_").Replace("\\", "_").Replace("/", "_").Replace("#", "_").Replace("\"", "_").Replace("'", "_") + extention;
	}
}


