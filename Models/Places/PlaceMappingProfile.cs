using AutoMapper;
using Models.ProcessingServices.EventsGrouping;

namespace Models.Places;

public class PlaceMappingProfile : Profile
{
    public PlaceMappingProfile()
    {
        CreateMap<Place, PlaceSynonyms>()
            .ForMember(placeSynonym => placeSynonym.Place, memberOptions => memberOptions.MapFrom(place => place.Name))
            .ForMember(placeSynonym => placeSynonym.Synonyms, memberOptions => 
                memberOptions.MapFrom(place => place.Synonyms.Select(synonym => synonym.Name)))
            ;
    }
}
