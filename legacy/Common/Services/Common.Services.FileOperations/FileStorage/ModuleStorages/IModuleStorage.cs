using System.Threading.Tasks;

namespace Common.Services.FileOperations.FileStorage;

public interface IModuleStorage
{
	string GetNormalizedFileName(string originalFileName, string extention);
	string GetDirectoryRelativePathForUploadFile(string basePath, string moduleName, Guid tenantId, string folderName, Guid? entityId, string versionName);
}