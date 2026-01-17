namespace Common.Services.Environment;

public class ModuleDefinitions
{
    // Base Modules
    public ModuleModel IamModule { get; }
    public ModuleModel ActivityLogModule { get; }
    public ModuleModel NotificationModule { get; }
    public ModuleModel FileProviderModule { get; }
    public ModuleModel DefinitionsModule { get; }

    // Business Modules
    public ModuleModel DocumentModule { get; }
    public ModuleModel ManagementModule { get; }
    public ModuleModel TodoModule { get; }
    public ModuleModel EbysModule { get; }
    public ModuleModel CisModule { get; }
    public ModuleModel KoopPosBackofficeModule { get; }
    public ModuleModel KoopPosStoreModule { get; }
    public ModuleModel IntranetModule { get; }
    public ModuleModel PdksModule { get; }
    public ModuleModel AutomateModule { get; }

    public ModuleDefinitions(string baseApiUrl, string baseUiUrl)
    {
        // BaseModules
        this.IamModule = new ModuleModel("IAM", "", baseApiUrl, baseUiUrl);
        this.ActivityLogModule = new ModuleModel("ActivityLog", "", baseApiUrl, baseUiUrl);
        this.ActivityLogModule = new ModuleModel("Notification", "", baseApiUrl, baseUiUrl);
        this.FileProviderModule = new ModuleModel("FileProvider", "", baseApiUrl, baseUiUrl);
        this.DefinitionsModule = new ModuleModel("Definitions", "", baseApiUrl, baseUiUrl);

        // Business Modules
        this.DocumentModule = new ModuleModel("Document", "mts", baseApiUrl, baseUiUrl);
        this.ManagementModule = new ModuleModel("Management", "sys", baseApiUrl, baseUiUrl);
        this.TodoModule = new ModuleModel("Todo", "todo", baseApiUrl, baseUiUrl);
        this.EbysModule = new ModuleModel("Ebys", "ebys", baseApiUrl, baseUiUrl);
        this.CisModule = new ModuleModel("Cis", "cis", baseApiUrl, baseUiUrl);
        this.KoopPosBackofficeModule = new ModuleModel("KoopPosBackoffice", "koopposbackoffice", baseApiUrl, baseUiUrl);
        this.KoopPosStoreModule = new ModuleModel("KoopPosStore", "koopposstore", baseApiUrl, baseUiUrl);
        this.IntranetModule = new ModuleModel("Intranet", "intranet", baseApiUrl, baseUiUrl);
        this.IntranetModule = new ModuleModel("Pdks", "pdks", baseApiUrl, baseUiUrl);
        this.AutomateModule = new ModuleModel("Automate", "automate", baseApiUrl, baseUiUrl);
    }
}