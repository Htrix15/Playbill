using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Playbill.Common.SearchOptions;
using Playbill.Services.EventDateIntervals.Common.Options;
using Playbill.Services.EventsGrouping;
using Playbill.Services.ExportEvents.ToHtml;
using Playbill.Services.TitleNormalization;

namespace Playbill.Infrastructure.Configure;

public static class Options
{
    public static void Configure(IServiceCollection services, IConfigurationRoot configuration)
    {
        services.Configure<SearchOptions>(configuration.GetSection("DefaultSearchOptions"));
        services.Configure<Billboards.Kassir.Options>(configuration.GetSection("Kassir"));
        services.Configure<Billboards.Ya.Options>(configuration.GetSection("Ya"));
        services.Configure<Billboards.Bezantracta.Options>(configuration.GetSection("Bezantracta"));
        services.Configure<Billboards.Quickticket.Options>(configuration.GetSection("Quickticket"));
        services.Configure<Billboards.Eventhall.Options>(configuration.GetSection("Eventhall"));
        services.Configure<Billboards.Ticketvrn.Options>(configuration.GetSection("Ticketvrn"));
        services.Configure<PlaceSynonymsOptions>(configuration.GetSection("PlaceSynonyms"));
        services.Configure<ExportToHtmlOptions>(configuration.GetSection("ExportToHtml"));
        services.Configure<TitleNormalizationOptions>(configuration.GetSection("TitleNormalizationOptions"));
        services.Configure<EventDateIntervalsOptions>(configuration.GetSection("Holidays"));
    }
}
