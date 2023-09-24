using Microsoft.Extensions.DependencyInjection;
using Models.Search;
using Models.ProcessingServices;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using ConsoleApp.ExportToHtml;

var customServices = new List<Action<ServiceCollection>>()
{
     services => services.AddTransient<ExportToHtmlService>()
};

var customConfigurations = new List<(string File, bool Optional)>()
{
    ("export-to-html-settings.json", false)
};

var customOptions = new List<Action<ServiceCollection, IConfigurationRoot>>()
{
    (services, configuration) => services.Configure<ExportToHtmlOptions>(configuration.GetSection("ExportToHtml"))
};

using var serviceProvider = Builder
    .GetServiceCollection(
        customServices: customServices,
        customConfigurations: customConfigurations,
        customOptions: customOptions
    )
    .BuildServiceProvider();

var events = await serviceProvider.GetService<MainService>()!.GetEvents(new SearchOptions()
    {
        DatePeriod = Models.ProcessingServices.EventDateIntervals.Common.Enums.DatePeriods.ThisMonth
    }, 
    overlayOptions: true
);
await serviceProvider.GetService<ExportToHtmlService>()!.ExportAync(events);

