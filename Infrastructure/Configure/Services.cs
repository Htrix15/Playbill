using Microsoft.Extensions.DependencyInjection;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Services.EventDateIntervals;

namespace Playbill.Infrastructure.Configure;

public static class Services
{
    public static IServiceCollection Configure()
    {
        return new ServiceCollection()
            .AddTransient<EventDateIntervalsService>()
            .AddTransient<IBillboardService, Billboards.Kassir.Service>()
            .AddTransient<IBillboardService, Billboards.Ya.Service>()
            .AddTransient<IBillboardService, Billboards.Bezantracta.Service>()
            .AddTransient<IBillboardService, Billboards.Quickticket.Service>()
            .AddTransient<IBillboardService, Billboards.Eventhall.Service>()
            .AddTransient<IBillboardService, Billboards.Ticketvrn.Service>()
            ;
    }
}
