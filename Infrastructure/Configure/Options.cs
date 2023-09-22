using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.Search;
using Models.ProcessingServices.EventDateIntervals.Common.Options;
using Models.ProcessingServices.ExportEvents.ToHtml;
using Models.ProcessingServices.TitleNormalization;
using Billboards = Models.Billboards;

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
        services.Configure<ExportToHtmlOptions>(configuration.GetSection("ExportToHtml"));
        services.Configure<TitleNormalizationOptions>(configuration.GetSection("TitleNormalizationOptions"));
        services.Configure<EventDateIntervalsOptions>(configuration.GetSection("Holidays"));
    }
}
