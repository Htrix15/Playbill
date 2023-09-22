using Infrastructure.Configure;
using Microsoft.Extensions.DependencyInjection;
using Playbill.Infrastructure.Configure;

namespace Infrastructure;

public class Builder
{
    public static IServiceCollection Build()
    {
        var services = Services.Configure();
        MappingProfiles.Configure(services);
        var configuration = Configurations.Configure();
        Options.Configure(services, configuration);
        Db.Configure(services);
        return services;
    }
}