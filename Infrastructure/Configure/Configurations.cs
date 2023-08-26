using Microsoft.Extensions.Configuration;

namespace Playbill.Infrastructure.Configure;

public static class Configurations
{
    public static IConfigurationRoot Configure()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("search-options.json", optional: false)
            .AddJsonFile("place-synonyms.json", optional: false)
            .AddJsonFile("billboards.json", optional: false)
            .AddJsonFile("title-normalization-settings.json", optional: false)
            .AddJsonFile("holidays.json", optional: false)
            .AddJsonFile("export-to-html-settings.json", optional: false)
            .Build();
    }
}
