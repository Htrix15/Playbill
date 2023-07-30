using Microsoft.Extensions.Configuration;

namespace Playbill.ConfigureInfrastructure;

public static class ConfigureConfigurations
{
    public static IConfigurationRoot Configure()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }
}
