using Microsoft.Extensions.Configuration;

namespace Playbill.Infrastructure.Configure;

public static class Configurations
{
    public static IConfigurationRoot Configure()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("place-synonyms.json", optional: false)
            .AddJsonFile("billboards.json", optional: false)
            .AddJsonFile("title-compare-settings.json", optional: false)
            .Build();
    }
}
