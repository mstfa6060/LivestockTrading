namespace BaseModules.FileProvider.Application.Configuration;

public class ProjectConfigurations : IConfigurationClass
{
    public RelationalDbConfiguration RelationalDbConfiguration { get; set; }
    public EnvironmentConfiguration EnvironmentConfiguration { get; set; }
    public DocumentDbOptions DocumentDbOptions { get; set; }
}