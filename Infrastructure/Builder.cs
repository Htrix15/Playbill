using Infrastructure.BaseConfigure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public class Builder
{

    private static void AddCustom(List<Action<ServiceCollection>>? actions, ServiceCollection services)
    {
        if (actions is not null && actions.Any())
        {
            actions.ForEach(addService =>
            {
                addService.Invoke(services);
            });
        }
    }

    private static void AddCustom<T>(List<Action<ServiceCollection, T>>? actions, ServiceCollection services, T options)
    {
        if (actions is not null && actions.Any())
        {
            actions.ForEach(addService =>
            {
                addService.Invoke(services, options);
            });
        }
    }

    private static void AddCustomConfiguration(List<(string File, bool Optional)>? customConfigurations, ConfigurationBuilder configurationBuilder)
    {
        if (customConfigurations is not null && customConfigurations.Any())
        {
            customConfigurations.ForEach(config =>
            {
                configurationBuilder.AddJsonFile(config.File, optional: config.Optional);
            });
        }
    }

    public static IServiceCollection GetServiceCollection(
        List<Action<ServiceCollection>>? customServices = null,
        List<(string File, bool Optional)>? customConfigurations = null,
        List<Action<ServiceCollection, IConfigurationRoot>>? customOptions = null,
        List<Action<ServiceCollection, IConfigurationRoot>>? customServicesWithOptions = null,
        List<Action<ServiceCollection>>? customDb = null,
        List<Action<ServiceCollection>>? customMapping = null,
        List<Action<ServiceCollection>>? customLogging = null)
    {
        var services = Services.GetCollection();
        AddCustom(customServices, services);

        var configuration = Configurations.GetBuilder();
        AddCustomConfiguration(customConfigurations, configuration);

        var configurationRoot = configuration.Build();

        Options.Configure(services, configurationRoot);
        AddCustom(customOptions, services, configurationRoot);

        AddCustom(customServicesWithOptions, services, configurationRoot);

        Db.Configure(services);
        AddCustom(customDb, services);

        MappingProfiles.Configure(services);
        AddCustom(customMapping, services);

        AddCustom(customLogging, services);

        return services;
    }
}