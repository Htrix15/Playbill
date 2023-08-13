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
            ;
    }
}
