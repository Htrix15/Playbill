using Microsoft.Extensions.DependencyInjection;
using Playbill.Services.EventDateIntervals;

namespace Playbill.Infrastructure.Configure;

public static class Services
{
    public static IServiceCollection Configure()
    {
        return new ServiceCollection()
            .AddTransient<EventDateIntervalsService>();
    }
}
