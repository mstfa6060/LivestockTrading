using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Common.Services.FileOperations.FileStorage;

public interface IFileStorageService
{
	Task<FileProperties> CreateFileByFormFile(Guid tenantId, IFormFile file, string moduleName, string folderName, Guid? entityId, string versionName);
	Task<FileProperties> CreateEypOfPdfFileByFormFile(Guid tenantId, IFormFile file, string fileName, string moduleName, string folderName, Guid? entityId, string versionName);
	Task<FileProperties> CreateFileByStream(Guid tenantId, Stream stream, string fileName, string contentType, string moduleName, string folderName, Guid? entityId, string versionName, bool overrideProhibited = false);
	Task<byte[]> ReadFileByPath(string path);
	Task<bool> WriteFileWithPath(string path, byte[] data);
	Task<FileProperties> DuplicateFile(Guid tenantId, FileProperties sourceFile, string moduleName, string folderName, Guid? entityId, string versionName);
	Task<bool> DeleteFile(string path);
	Task<FileProperties> CreateUserImage(Guid tenantId, string fileName, string extention, byte[] data);
	Task<FileProperties> CreateGroupImage(Guid tenantId, string fileName, string extention, byte[] data);
	string GetBasePath();
	Stream GetFileStream(string filePath);
	(FileProperties Properties, string PhysicalFilePath) BuildFileProperties(Guid tenantId, string _fileName, string contentType, string moduleName, string folderName, Guid? entityId, string versionName);
}