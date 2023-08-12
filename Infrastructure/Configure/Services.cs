using Microsoft.Extensions.DependencyInjection;

namespace Playbill.Infrastructure.Configure;

public static class Services
{
    public static IServiceCollection Configure()
    {
        return new ServiceCollection();
    }
}
