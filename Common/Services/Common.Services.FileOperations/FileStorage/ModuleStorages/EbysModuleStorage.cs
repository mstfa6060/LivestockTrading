namespace Common.Services.FileOperations.FileStorage;

public class EbysModuleStorage : IModuleStorage
{
    public EbysModuleStorage()
    { }

    public string GetDirectoryRelativePathForUploadFile(string basePath, string moduleName, Guid tenantId, string folderName, Guid? entityId, string versionName)
    {
        var normalizedEntityId = entityId?.ToString() ?? Guid.Empty.ToString();
        var folderSplitter = normalizedEntityId.Split("-")[2];

        string newPathName;

        /*
		templates:
		/home/data/files/modules/ebys/b2ad6f15-b7d8-40e3-9262-d9ff9cbd37d2/templates/2b4334c0-1ea7-491c-bac1-4a61a00b69ad/bir_imzali.docx
		document:
		/home/data/files/modules/ebys/shared/documents/internal/491c/2b4334c0-1ea7-491c-bac1-4a61a00b69ad/1/Attachments/bir_imzali.docx
		companyTemplates:
		/home/data/files/modules/ebys/companyTemplates/8A779814-660A-4AF5-B775-5991DF4AB7BF.docx
		defaultTemplate:
		/home/data/files/modules/ebys/defaultTemplates/template.docx
		*/

        switch (folderName)
        {
            case "templates":
                newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), normalizedEntityId, folderName);
                break;

            case "attachments":
                newPathName = Path.Combine(basePath, "modules", moduleName, "shared", "documents", "internal", folderSplitter, normalizedEntityId, versionName, folderName);
                break;

            case "documents":
                newPathName = Path.Combine(basePath, "modules", moduleName, "shared", "documents", "internal", folderSplitter, normalizedEntityId, versionName);
                break;

            case "companyTemplates":
                newPathName = Path.Combine(basePath, "modules", moduleName, folderName);
                break;

            case "defaultTemplates":
                newPathName = Path.Combine(basePath, "modules", moduleName, folderName);
                break;

            default:
                throw new Exception("FolderName not handled");
        }

        if (!Directory.Exists(newPathName))
            Directory.CreateDirectory(newPathName);

        newPathName = newPathName.Replace(basePath, "");
        newPathName = newPathName.StartsWith("/") ? newPathName.Substring(1, newPathName.Length - 1) : newPathName;

        return newPathName;
    }

    public string GetNormalizedFileName(string originalFileName, string extention)
    {
        //TODO: %, [,], *, <,>, linux unexcepted + eyp unexcepted chars replace with _
        return originalFileName.Replace(" ", "_")
                                .Replace(".", "_")
                                .Replace(",", "_")
                                .Replace("(", "")
                                .Replace(")", "")
                                .Replace("%", "_")
                                .Replace("[", "_")
                                .Replace("]", "_")
                                .Replace("*", "_")
                                .Replace("<", "_")
                                .Replace(">", "_")
                                .Replace("$", "_")
                                .Replace("@", "_")
                                .Replace("^", "_")
                                .Replace("₺", "_")
                                .Replace("\\", "_")
                                .Replace("/", "_")
                                .Replace("#", "_")
                                .Replace("\"", "_")
                                .Replace("'", "_") + extention;
    }
}


