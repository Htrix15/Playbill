using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Playbill.ConfigureInfrastructure;

public static class ConfigureOptions
{
    public static void Configure(IServiceCollection services, IConfigurationRoot configuration)
    {
      //sample  services.Configure<Config>(configuration.GetSection("Config"));
    }
}
