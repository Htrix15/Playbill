using Microsoft.Extensions.DependencyInjection;
using Playbill.Infrastructure.Configure;
using Playbill.Services;
using Playbill.Services.ExportEvents.ToHtml;

var services = Services.Configure();
var configuration = Configurations.Configure();
Options.Configure(services, configuration);

using var serviceProvider = services.BuildServiceProvider();

var events = await serviceProvider.GetService<MainService>().GetEvents(new Playbill.Common.SearchOptions.SearchOptions());
await serviceProvider.GetService<ExportToHtmlService>().ExportAync(events);


