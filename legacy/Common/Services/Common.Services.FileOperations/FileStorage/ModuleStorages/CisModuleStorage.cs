namespace Common.Services.FileOperations.FileStorage;

public class CisModuleStorage : IModuleStorage
{
	public CisModuleStorage()
	{ }

	public string GetDirectoryRelativePathForUploadFile(string basePath, string moduleName, Guid tenantId, string folderName, Guid? entityId, string versionName)
	{
		var normalizedEntityId = entityId?.ToString() ?? Guid.Empty.ToString();

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
			case "organizationSchemes":
			case "valuableDocuments":
			case "boardStructures":
			case "balances":
			case "licenses":
			case "boardDecisions":
			case "disciplines":
			case "inspections":
			case "committees":
			case "gdprCouncils":
				newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), folderName, entityId.ToString());
				// newPathName = Path.Combine(basePath, "modules", moduleName, "shared", folderSplitter, normalizedEntityId, folderName);
				break;
			case "boardDecisionAttachments":
				newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), "boardDecisions", entityId.ToString(), folderName);
				// newPathName = Path.Combine(basePath, "modules", moduleName, "shared", folderSplitter, normalizedEntityId, "boardDecisions", folderName);
				break;
			case "inspectionAttachments":
				newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), "inspections", entityId.ToString(), folderName);
				// newPathName = Path.Combine(basePath, "modules", moduleName, "shared", folderSplitter, normalizedEntityId, "boardDecisions", folderName);
				break;
			case "committeeAttachments":
				newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), "committees", entityId.ToString(), folderName);
				// newPathName = Path.Combine(basePath, "modules", moduleName, "shared", folderSplitter, normalizedEntityId, "boardDecisions", folderName);
				break;
			case "gdprCouncilAttachments":
				newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), "gdprCouncils", entityId.ToString(), folderName);
				// newPathName = Path.Combine(basePath, "modules", moduleName, "shared", folderSplitter, normalizedEntityId, "boardDecisions", folderName);
				break;
			case "boardDecisionMotions":
				newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), "boardDecisions", entityId.ToString(), folderName);
				// newPathName = Path.Combine(basePath, "modules", moduleName, "shared", folderSplitter, normalizedEntityId, "boardDecisions", folderName);
				break;
			case "disciplineAttachments":
				newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), "disciplines", entityId.ToString(), folderName);
				// newPathName = Path.Combine(basePath, "modules", moduleName, "shared", folderSplitter, normalizedEntityId, "boardDecisions", folderName);
				break;
			case "balanceAttachments":
				newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), "balances", entityId.ToString(), folderName);
				// newPathName = Path.Combine(basePath, "modules", moduleName, "shared", folderSplitter, normalizedEntityId, "boardDecisions", folderName);
				break;
			case "companyInformation":
				newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), "logos");
				// newPathName = Path.Combine(basePath, "modules", moduleName, "shared", folderSplitter, normalizedEntityId, folderName);
				break;
			case "companyIdentities":
				newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), folderName);
				// newPathName = Path.Combine(basePath, "modules", moduleName, "shared", folderSplitter, normalizedEntityId, folderName);
				break;
			case "balanceTemplates":
				newPathName = Path.Combine(basePath, "modules", moduleName, tenantId.ToString(), folderName);
				// newPathName = Path.Combine(basePath, "modules", moduleName, "shared", folderSplitter, normalizedEntityId, folderName);
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
		originalFileName = string.Concat(Guid.NewGuid().ToString().Replace("-", ""), "_", originalFileName);
		//TODO: %, [,], *, <,>, linux unexcepted + eyp unexcepted chars replace with _
		return originalFileName.Replace(" ", "_").Replace(".", "_").Replace("(", "").Replace(")", "").Replace("%", "_").Replace("[", "_").Replace("]", "_").Replace("*", "_").Replace("<", "_").Replace(">", "_").Replace("$", "_").Replace("@", "_").Replace("^", "_").Replace("₺", "_").Replace("\\", "_").Replace("/", "_").Replace("#", "_").Replace("\"", "_").Replace("'", "_") + extention;
	}
}


