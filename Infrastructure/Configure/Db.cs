using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Places;
using Repository;

namespace Playbill.Infrastructure.Configure;

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
