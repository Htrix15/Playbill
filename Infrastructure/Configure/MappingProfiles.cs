using Microsoft.Extensions.DependencyInjection;
using Models.Places;
using Models.Search;

namespace Playbill.Infrastructure.Configure;

public static class MappingProfiles
{
    public static void Configure(IServiceCollection services)
    {
        services
            .AddAutoMapper(typeof(SearchOptionsMappingProfile))
            .AddAutoMapper(typeof(PlaceMappingProfile))
            ;
    }
}
