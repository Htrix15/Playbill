using Microsoft.Extensions.DependencyInjection;
using Models.Search;
using Models.ProcessingServices;
using Models.ProcessingServices.ExportEvents.ToHtml;
using Infrastructure;

using var serviceProvider = Builder.Build().BuildServiceProvider();
var events = await serviceProvider.GetService<MainService>().GetEvents(new SearchOptions());
await serviceProvider.GetService<ExportToHtmlService>().ExportAync(events);

