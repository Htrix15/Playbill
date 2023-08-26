using AutoMapper;

namespace Playbill.Common.SearchOptions;

public class SearchOptionsMappingProfile : Profile
{
    public SearchOptionsMappingProfile()
    {
        AllowNullCollections = true;
        CreateMap<SearchOptions, SearchOptions>()
            .ForAllMembers(opts => opts.Condition((source, destination, member) => member != null))
        ;
    }
}

