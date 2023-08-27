using Microsoft.Extensions.DependencyInjection;
using Models.Search;
using Models.ProcessingServices;
using Models.ProcessingServices.ExportEvents.ToHtml;
using Playbill.Infrastructure.Configure;

var services = Services.Configure();
MappingProfiles.Configure(services);
var configuration = Configurations.Configure();
Options.Configure(services, configuration);
Db.Configure(services);

using var serviceProvider = services.BuildServiceProvider();

var events = await serviceProvider.GetService<MainService>().GetEvents(new SearchOptions());
await serviceProvider.GetService<ExportToHtmlService>().ExportAync(events);

