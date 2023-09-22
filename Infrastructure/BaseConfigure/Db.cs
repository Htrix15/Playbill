using Microsoft.Extensions.DependencyInjection;
using Models.Places;
using Repository;

namespace Infrastructure.BaseConfigure;

public static class Db
{
    public static void Configure(IServiceCollection services)
    {
        services
            .AddDbContext<ApplicationDbContext>()
            .AddTransient<IPlaceRepository, PlaceRespository>()
            ;
    }       
}
