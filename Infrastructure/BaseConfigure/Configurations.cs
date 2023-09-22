using Microsoft.Extensions.Configuration;

namespace Infrastructure.BaseConfigure;

public static class Configurations
{
    public static ConfigurationBuilder GetBuilder()
    {
        var builder = new ConfigurationBuilder();

        builder
            .AddJsonFile("search-options.json", optional: false)
            .AddJsonFile("billboards.json", optional: false)
            .AddJsonFile("title-normalization-settings.json", optional: false)
            .AddJsonFile("holidays.json", optional: false)
        ;

        return builder;
    }
}
    