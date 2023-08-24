using Microsoft.Extensions.DependencyInjection;
using Playbill.Infrastructure.Configure;
using Playbill.Services.EventDateIntervals;
using Playbill.Services.EventsGrouping;
using Playbill.Services.ExportEvents.ToHtml;
using Playbill.Services.LoadEvents;
using DatePeriods = Playbill.Services.EventDateIntervals.Common.Enums.DatePeriods;

var services = Services.Configure();
var configuration = Configurations.Configure();
Options.Configure(services, configuration);

using var serviceProvider = services.BuildServiceProvider();
var intervals = serviceProvider.GetService<EventDateIntervalsService>()
    .GetDateIntervals(new HashSet<DayOfWeek>(), // { DayOfWeek.Saturday, DayOfWeek.Sunday },
    DatePeriods.ThisYear);
var events = await serviceProvider.GetService<LoadEventsService>().GetEventsAsync(intervals);
events = serviceProvider.GetService<EventsGroupingService>().EventsGrouping(events);
await serviceProvider.GetService<ExportToHtmlService>().ExportAync(events);


