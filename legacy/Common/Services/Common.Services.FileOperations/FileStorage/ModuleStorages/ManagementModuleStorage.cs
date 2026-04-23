namespace Common.Services.FileOperations.FileStorage;

public class ManagementModuleStorage : IModuleStorage
{
	public ManagementModuleStorage()
	{ }

	public string GetDirectoryRelativePathForUploadFile(string basePath, string moduleName, Guid tenantId, string folderName, Guid? entityId, string versionName)
	{
		var day = DateTime.Now.Day;
		var month = DateTime.Now.Month;
		var dayName = (day < 10) ? $"0{day}" : day.ToString();
		var monthName = (month < 10) ? $"0{month}" : month.ToString();
		var yearName = DateTime.Now.Year.ToString();

		var tenantPath = Path.Combine(basePath, tenantId.ToString());
		var newPathName = Path.Combine(tenantPath, "uploads", moduleName);
		if (!Directory.Exists(newPathName))
			Directory.CreateDirectory(newPathName);

		newPathName = Path.Combine(newPathName, yearName);
		if (!Directory.Exists(newPathName))
			Directory.CreateDirectory(newPathName);

		newPathName = Path.Combine(newPathName, monthName);
		if (!Directory.Exists(newPathName))
			Directory.CreateDirectory(newPathName);

		newPathName = Path.Combine(newPathName, dayName);
		if (!Directory.Exists(newPathName))
			Directory.CreateDirectory(newPathName);

		newPathName = newPathName.Replace(basePath, "");
		newPathName = newPathName.StartsWith("/") ? newPathName.Substring(1, newPathName.Length - 1) : newPathName;

		return newPathName;
	}

	public string GetNormalizedFileName(string originalFileName, string extention)
	{
		return originalFileName.Replace(" ", "_") + "-" + Guid.NewGuid().ToString().Replace("-", "") + extention;
	}
}


