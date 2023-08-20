using Microsoft.Extensions.DependencyInjection;
using Playbill.Infrastructure.Configure;
using Playbill.Services.EventDateIntervals;
using Playbill.Services.EventsGrouping;
using Playbill.Services.LoadEvents;
using PresetEventDateIntervals = Playbill.Services.EventDateIntervals.Common.Enums.PresetEventDateIntervals;

var services = Services.Configure();
var configuration = Configurations.Configure();
Options.Configure(services, configuration);

using var serviceProvider = services.BuildServiceProvider();
var intervals = serviceProvider.GetService<EventDateIntervalsService>().GetEventDateInterval(PresetEventDateIntervals.NextWeek);// startDate: DateTime.Now, endDate: new DateTime(2023, 12, 31)
var events = await serviceProvider.GetService<LoadEventsService>().GetEventsAsync(intervals);
events = serviceProvider.GetService<EventsGroupingService>().EventsGrouping(events);


