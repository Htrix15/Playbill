using AutoMapper;
using Models.Events;
using Models.ProcessingServices.EventsGrouping;
using System.Text.RegularExpressions;

namespace Models.Places;

public class PlacesService
{
    private readonly IMapper _mapper;
    private readonly IPlaceRepository _placeRepository;

    public PlacesService(IMapper mapper, IRepository<Place> placeRepository)
    {
        _mapper = mapper;
        _placeRepository = (IPlaceRepository)placeRepository;
    }

    public async Task<List<Place>> GetPlacesAsync() => await _placeRepository.GetPlacesAsync();

    public List<PlaceSynonyms> GetPlaceSynonyms(List<Place> places) 
        => _mapper.Map<List<PlaceSynonyms>>(places.Where(place => place.ParentId is null && place.Synonyms.Any()));

    public void ReplacePlaceToSynonyms(IList<Event> events, List<PlaceSynonyms> placesSynonyms)
    {
        foreach (var @event in events)
        {
            @event.Place = Regex.Replace(@event?.Place ?? "", @"\s+", " ");
            placesSynonyms.ForEach(synonyms => synonyms.SetPlaceName(@event));
        }
    }

    public async Task AddNewPlaceAsync(List<Place> places, IList<Event> events)
    {
        var allPlacesNames = places.SelectMany(place => place.Synonyms.Select(synonym => synonym.Name)).ToList();
        allPlacesNames.AddRange(places.Select(place => place.Name));

        var eventPlacesNames = events.Where(@event => !@event.SplitPlace).Select(@event => @event.Place).Distinct().ToList();
        eventPlacesNames.AddRange(events.Where(@event => @event.SplitPlace).SelectMany(@event => @event.Place.Split(" | ")));
        eventPlacesNames = eventPlacesNames.Distinct().ToList();
        var newPlacesNames = eventPlacesNames.Except(allPlacesNames);
        if (newPlacesNames.Any())
        {
            await _placeRepository.AddPlaceAsync(newPlacesNames);
        }
    }
}
