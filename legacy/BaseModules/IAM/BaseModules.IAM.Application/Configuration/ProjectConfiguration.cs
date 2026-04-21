
namespace BaseModules.IAM.Application.Configuration;

public class ProjectConfigurations : IConfigurationClass
{
	public RelationalDbConfiguration RelationalDbConfiguration { get; set; }
	public EnvironmentConfiguration EnvironmentConfiguration { get; set; }
	public ExternalAuthConfiguration ExternalAuth { get; set; }
}