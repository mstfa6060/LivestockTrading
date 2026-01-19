namespace LivestockTrading.Application.Configuration;

public class ProjectConfigurations : IConfigurationClass
{
    public RelationalDbConfiguration RelationalDbConfiguration { get; set; }
    public EnvironmentConfiguration EnvironmentConfiguration { get; set; }
}
