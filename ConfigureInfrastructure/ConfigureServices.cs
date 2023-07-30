using Microsoft.Extensions.DependencyInjection;
using Playbill.Billboards.Sample;

namespace Playbill.ConfigureInfrastructure;

public static class ConfigureServices
{
    public static IServiceCollection Configure()
    {
        return new ServiceCollection()
            .AddTransient<SampleService>();
    }
}
