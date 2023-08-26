using Microsoft.Extensions.DependencyInjection;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Common.SearchOptions;
using Playbill.Services;
using Playbill.Services.EventDateIntervals;
using Playbill.Services.EventsGrouping;
using Playbill.Services.ExportEvents.ToHtml;
using Playbill.Services.FilterEvents;
using Playbill.Services.LoadEvents;
using Playbill.Services.TitleNormalization;
using Playbill.Services.TitleNormalization.Common;

namespace Playbill.Infrastructure.Configure;

public static class Services
{
    public static IServiceCollection Configure()
    {
        return new ServiceCollection()
            .AddAutoMapper(typeof(SearchOptionsMappingProfile))
            .AddTransient<MainService>()
            .AddTransient<EventDateIntervalsService>()
            .AddTransient<LoadEventsService>()
            .AddTransient<ITitleNormalization, TitleNormalizationService>()
            .AddTransient<IBillboardService, Billboards.Kassir.Service>()
            .AddTransient<IBillboardService, Billboards.Ya.Service>()
            .AddTransient<IBillboardService, Billboards.Bezantracta.Service>()
            .AddTransient<IBillboardService, Billboards.Quickticket.Service>()
            .AddTransient<IBillboardService, Billboards.Eventhall.Service>()
            .AddTransient<IBillboardService, Billboards.Ticketvrn.Service>()
            .AddTransient<FilterEventsService>()
            .AddTransient<EventsGroupingService>()
            .AddTransient<ExportToHtmlService>()
        ;
    }
}
