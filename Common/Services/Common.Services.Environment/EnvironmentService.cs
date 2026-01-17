namespace Common.Services.Environment;

public class EnvironmentService
{
    public string EnvironmentName { get; }
    public CustomEnvironments Environment { get; }
    public string ApiUrl { get; }
    public string UiUrl { get; }
    public string PttKepServiceUrl { get; set; }
    public string MerkezBirligiServiceUrl { get; set; }
    public ModuleDefinitions Modules { get; }

    public EnvironmentService(EnvironmentConfiguration configuration)
    {
        switch (configuration.EnvironmentName)
        {
            case "Production":
                this.Environment = CustomEnvironments.Production;
                this.ApiUrl = "";
                this.UiUrl = "";
                this.PttKepServiceUrl = "https://ws.hs01.kep.tr/KepEYazismaV1.1/KepEYazismaCOREWSDL.php";
                this.MerkezBirligiServiceUrl = "";
                break;
            case "Staging":
                this.Environment = CustomEnvironments.Staging;
                this.ApiUrl = "";
                this.UiUrl = "";
                this.PttKepServiceUrl = "https://eyazisma.hs01.kep.tr/KepEYazismaV1.1/KepEYazismaCOREWSDL.php";
                this.MerkezBirligiServiceUrl = "http://localhost:44364";
                break;
            case "Development":
                this.Environment = CustomEnvironments.Development;
                this.ApiUrl = "http://localhost:5257";
                this.PttKepServiceUrl = "https://eyazisma.hs01.kep.tr/KepEYazismaV1.1/KepEYazismaCOREWSDL.php";
                this.MerkezBirligiServiceUrl = "http://localhost:44364";
                break;
            case "Test":
                this.Environment = CustomEnvironments.Test;
                this.ApiUrl = "no-url";
                this.UiUrl = "no-url";
                break;
        }

        this.EnvironmentName = configuration.EnvironmentName;
        this.ApiUrl = BaseUrls.GetApiUrl(this.Environment);
        this.UiUrl = BaseUrls.GetUiUrl(this.Environment);
        this.Modules = new ModuleDefinitions(this.ApiUrl, this.UiUrl);
    }
}