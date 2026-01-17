namespace Common.Services.Environment;

public class ModuleModel
{
	public string Name { get; }
	public string UiUrl { get; }
	public string ApiUrl { get; }

	public ModuleModel(string name, string urlAcronym, string baseApiUrl, string baseUiUrl)
	{
		this.Name = name;
		this.ApiUrl = $"{baseApiUrl}/{this.Name.ToLower()}";
		this.UiUrl = $"{baseUiUrl}/_{urlAcronym}";
	}
}