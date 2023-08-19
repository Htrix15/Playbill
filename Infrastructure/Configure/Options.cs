using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Playbill.Billboards.Common.Options;

namespace Playbill.Infrastructure.Configure;

public static class Options
{
    public static void Configure(IServiceCollection services, IConfigurationRoot configuration)
    {
        services.Configure<SupportedBillboardTypesOptions>(configuration.GetSection("Billboards"));
        services.Configure<Billboards.Kassir.Options>(configuration.GetSection("Kassir"));
        services.Configure<Billboards.Ya.Options>(configuration.GetSection("Ya"));
    }
}
