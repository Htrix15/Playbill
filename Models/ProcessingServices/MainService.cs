using AutoMapper;
using Microsoft.Extensions.Options;
using Models.Events;
using Models.Search;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.EventsGrouping;
using Models.ProcessingServices.FilterEvents;
using Models.ProcessingServices.LoadEvents;
using Models.Places;

namespace Models.ProcessingServices;

public class MainService
{
    private readonly SearchOptions _searchOptions;

    private readonly IMapper _mapper;

    private readonly EventDateIntervalsService _eventDateIntervalsService;
    private readonly LoadEventsService _loadEventsService;
    private readonly EventsGroupingService _eventsGroupingService;
    private readonly FilterEventsService _filterEventsService;
    private readonly PlacesService _placesService;

    public MainService(IOptions<SearchOptions> defaultOptions, 
        IMapper mapper,
        EventDateIntervalsService eventDateIntervalsService,
        LoadEventsService loadEventsService,
        EventsGroupingService eventsGroupingService,
        FilterEventsService filterEventsService,
        PlacesService placesService)
    {
        _searchOptions = defaultOptions.Value;
        _mapper = mapper;
        _eventDateIntervalsService = eventDateIntervalsService;
        _loadEventsService = loadEventsService;
        _eventsGroupingService = eventsGroupingService;
        _filterEventsService = filterEventsService;
        _placesService = placesService;
    }

    private SearchOptions OverlayOptions(SearchOptions userSearchOptions)
    {
       return _mapper.Map(userSearchOptions, _searchOptions.Clone() as SearchOptions);
    }

    public async Task<IList<Event>> GetEvents(SearchOptions userSearchOptions, bool overlayOptions = false)
    {
        var places = await _placesService.GetPlacesAsync();
        var placesSynonyms = _placesService.GetPlaceSynonyms(places);
        
        if (overlayOptions) { 
            userSearchOptions = OverlayOptions(userSearchOptions);
        }

        var intervals = await _eventDateIntervalsService
            .GetDateIntervalsAsync(userSearchOptions.DaysOfWeek!,
            userSearchOptions?.DatePeriod ?? EventDateIntervals.Common.Enums.DatePeriods.ThisWeek,
            userSearchOptions.StartDate,
            userSearchOptions.EndDate,
            userSearchOptions.AddHolidays ?? false);

        var events = await _loadEventsService.GetEventsAsync(userSearchOptions.SupportedBillboards!, 
            intervals,
            userSearchOptions.SearchEventTypes!);

        _placesService.ReplacePlaceToSynonyms(events, placesSynonyms);

        events = _filterEventsService.FilterEvents(events, userSearchOptions.AllPlaces,
            userSearchOptions?.ExcludePlacesTerms ?? new HashSet<string>(),
            places.Select(p => p.Name).ToHashSet(),
            userSearchOptions?.ExcludeEventsNamesTerms ?? new Dictionary<EventTypes, HashSet<string>>());

        events = _eventsGroupingService.EventsGrouping(events);

        events = events.Where(@event => intervals.Any(interval => DateOnly.FromDateTime(@event.Date.Value) >= interval.StartDate && DateOnly.FromDateTime(@event.Date.Value) <= interval.EndDate)).ToList();

        await _placesService.AddNewPlaceAsync(places, events);

        return events;
    }
}
