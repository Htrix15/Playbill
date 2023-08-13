using Microsoft.Extensions.DependencyInjection;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Infrastructure.Configure;
using Playbill.Services.EventDateIntervals;
using PresetEventDateIntervals = Playbill.Services.EventDateIntervals.Common.Enums.PresetEventDateIntervals;

var services = Services.Configure();
var configuration = Configurations.Configure();
Options.Configure(services, configuration);

using var serviceProvider = services.BuildServiceProvider();
var intervals = serviceProvider.GetService<EventDateIntervalsService>().GetEventDateInterval(PresetEventDateIntervals.NextWeekWeekend);
await serviceProvider.GetService<IBillboardService>().GetEventsAsync(intervals);

