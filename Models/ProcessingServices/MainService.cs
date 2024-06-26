﻿using AutoMapper;
using Microsoft.Extensions.Options;
using Models.Events;
using Models.Search;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.EventsGrouping;
using Models.ProcessingServices.FilterEvents;
using Models.ProcessingServices.LoadEvents;
using Models.Places;

namespace Models.ProcessingServices;

public class MainService(IOptions<SearchOptions> defaultOptions,
    IMapper mapper,
    EventDateIntervalsService eventDateIntervalsService,
    LoadEventsService loadEventsService,
    EventsGroupingService eventsGroupingService,
    FilterEventsService filterEventsService,
    PlacesService placesService)
{
    private readonly SearchOptions _searchOptions = defaultOptions.Value;

    private readonly IMapper _mapper = mapper;

    private readonly EventDateIntervalsService _eventDateIntervalsService = eventDateIntervalsService;
    private readonly LoadEventsService _loadEventsService = loadEventsService;
    private readonly EventsGroupingService _eventsGroupingService = eventsGroupingService;
    private readonly FilterEventsService _filterEventsService = filterEventsService;
    private readonly PlacesService _placesService = placesService;

    private SearchOptions OverlayOptions(SearchOptions userSearchOptions)
    {
       return _mapper.Map(userSearchOptions, _searchOptions.Clone() as SearchOptions);
    }

    public async Task<IList<Event>> GetEvents(SearchOptions userSearchOptions, bool overlayOptions = false)
    {
        var places = await _placesService.GetPlacesAsync();
        var placesSynonyms = _placesService.GetPlaceSynonyms(places);

        if (overlayOptions)
        {
            userSearchOptions = OverlayOptions(userSearchOptions);
        }
        if (userSearchOptions.ExcludeBillboards?.Any() ?? false)
        {
            userSearchOptions.SupportedBillboards = userSearchOptions.SupportedBillboards.Except(_searchOptions.ExcludeBillboards).ToHashSet();
        }
        if (userSearchOptions.ExcludeSearchEventTypes?.Any() ?? false)
        {
            userSearchOptions.SearchEventTypes = userSearchOptions.SearchEventTypes.Except(_searchOptions.ExcludeSearchEventTypes).ToHashSet();
        }

        var intervals = await _eventDateIntervalsService
            .GetDateIntervalsAsync(userSearchOptions.DaysOfWeek!,
            userSearchOptions?.DatePeriod ?? EventDateIntervals.Common.Enums.DatePeriods.ThisWeek,
            userSearchOptions.StartDate,
            userSearchOptions.EndDate,
            userSearchOptions.AddHolidays ?? false);

        var eventResults = await _loadEventsService.GetEventsAsync(userSearchOptions.SupportedBillboards!, 
            intervals,
            userSearchOptions.SearchEventTypes!);

        var events = eventResults.SelectMany(e => e.Result).ToList();
        var substandardEvents = eventResults.SelectMany(e => e.SubstandardEvents).ToList();

        _placesService.ReplacePlaceToSynonyms(events, placesSynonyms);
        _placesService.ReplacePlaceToSynonyms(substandardEvents, placesSynonyms);

        events = _filterEventsService.FilterEvents(events, userSearchOptions.AllPlaces,
            userSearchOptions?.ExcludePlacesTerms ?? new HashSet<string>(),
            places.Select(p => p.Name).ToHashSet(),
            userSearchOptions?.ExcludeEventsNamesTerms ?? new Dictionary<EventTypes, HashSet<string>>());

        events = _eventsGroupingService.EventsGrouping(events);

        events = events.Where(@event => intervals.Any(interval => DateOnly.FromDateTime(@event.Date.Value) >= interval.StartDate && DateOnly.FromDateTime(@event.Date.Value) <= interval.EndDate)).ToList();

        events.AddRange(substandardEvents);

        await _placesService.AddNewPlaceAsync(places, events);

        return events;
    }
}
