using Microsoft.Extensions.DependencyInjection;

namespace Playbill.ConfigureInfrastructure;

public static class ConfigureServices
{
    public static IServiceCollection Configure()
    {
        return new ServiceCollection();
            //sample 
            //.AddTransient<ITest, TestService>()
            //.AddTransient<Test2Service>();
    }
}
