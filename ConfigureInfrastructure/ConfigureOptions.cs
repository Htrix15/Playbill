using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Playbill.Billboards.Sample;

namespace Playbill.ConfigureInfrastructure;

public static class ConfigureOptions
{
    public static void Configure(IServiceCollection services, IConfigurationRoot configuration)
    {
      services.Configure<SampleApiOptions>(configuration.GetSection("SampleApi"));
      services.Configure<SamplePageParseOptions>(configuration.GetSection("SamplePageParse"));
    }
}
