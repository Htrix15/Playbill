using AutoMapper;
using Microsoft.Extensions.Options;
using Models.Events;
using Models.Search;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.EventsGrouping;
using Models.ProcessingServices.FilterEvents;
using Models.ProcessingServices.LoadEvents;
using Models.Places;
using System.Text.RegularExpressions;

namespace Models.ProcessingServices;

public class MainService
{
    private SearchOptions _searchOptions;

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

    private void OverlayOptions(SearchOptions userSearchOptions)
    {
        _mapper.Map(userSearchOptions, _searchOptions);
    }

    public async Task<IList<Event>> GetEvents(SearchOptions userSearchOptions)
    {
        var places = await _placesService.GetPlacesAsync();
        var placesSynonyms = _placesService.GetPlaceSynonyms(places);

        OverlayOptions(userSearchOptions);

        var intervals = await _eventDateIntervalsService
            .GetDateIntervalsAsync(_searchOptions.DaysOfWeek!,
            _searchOptions!.DatePeriod,
            _searchOptions.StartDate,
            _searchOptions.EndDate,
            _searchOptions.AddHolidays ?? false);

        var events = await _loadEventsService.GetEventsAsync(_searchOptions.SupportedBillboards!, 
            intervals, 
            _searchOptions.SearchEventTypes!);

        _placesService.ReplacePlaceToSynonyms(events, placesSynonyms);

        events = _filterEventsService.FilterEvents(events, _searchOptions.AllPlaces, 
            _searchOptions.ExcludePlacesTerms!,
            places.Select(p => p.Name).ToHashSet(), 
            _searchOptions.ExcludeEventsNamesTerms);

        events = _eventsGroupingService.EventsGrouping(events);

        await _placesService.AddNewPlaceAsync(places, events);

        return events;
    }
}
