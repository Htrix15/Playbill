using Microsoft.Extensions.DependencyInjection;
using Playbill.Infrastructure.Configure;
using Playbill.Services.EventDateIntervals;
using Playbill.Services.LoadEvents;
using PresetEventDateIntervals = Playbill.Services.EventDateIntervals.Common.Enums.PresetEventDateIntervals;

var services = Services.Configure();
var configuration = Configurations.Configure();
Options.Configure(services, configuration);

using var serviceProvider = services.BuildServiceProvider();
var intervals = serviceProvider.GetService<EventDateIntervalsService>().GetEventDateInterval(PresetEventDateIntervals.Next30Days);
await serviceProvider.GetService<LoadEventsService>().GetEventsAsync(intervals);

