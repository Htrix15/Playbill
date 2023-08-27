using Microsoft.Extensions.DependencyInjection;
using Models.Billboards.Common.Interfaces;
using Models.Places;
using Models.ProcessingServices;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.EventsGrouping;
using Models.ProcessingServices.ExportEvents.ToHtml;
using Models.ProcessingServices.FilterEvents;
using Models.ProcessingServices.LoadEvents;
using Models.ProcessingServices.TitleNormalization;
using Models.ProcessingServices.TitleNormalization.Common;
using Billboards = Models.Billboards;

namespace Playbill.Infrastructure.Configure;

public static class Services
{
    public static IServiceCollection Configure()
    {
        return new ServiceCollection()
            .AddTransient<MainService>()
            .AddTransient<PlacesService>()
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
