using Microsoft.Extensions.DependencyInjection;
using Models.Places;
using Models.Search;

namespace Infrastructure.BaseConfigure;

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
