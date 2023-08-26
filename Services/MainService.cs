using AutoMapper;
using Microsoft.Extensions.Options;
using Playbill.Common.Event;
using Playbill.Common.SearchOptions;
using Playbill.Services.EventDateIntervals;
using Playbill.Services.EventsGrouping;
using Playbill.Services.FilterEvents;
using Playbill.Services.LoadEvents;

namespace Playbill.Services;

public class MainService
{
    private SearchOptions _searchOptions;

    private readonly IMapper _mapper;

    private readonly EventDateIntervalsService _eventDateIntervalsService;
    private readonly LoadEventsService _loadEventsService;
    private readonly EventsGroupingService _eventsGroupingService;
    private readonly FilterEventsService _filterEventsService;

    public MainService(IOptions<SearchOptions> defaultOptions,
                IMapper mapper,
        EventDateIntervalsService eventDateIntervalsService,
        LoadEventsService loadEventsService,
        EventsGroupingService eventsGroupingService,
        FilterEventsService filterEventsService)
    {
        _searchOptions = defaultOptions.Value;
        _mapper = mapper;
        _eventDateIntervalsService = eventDateIntervalsService;
        _loadEventsService = loadEventsService;
        _eventsGroupingService = eventsGroupingService;
        _filterEventsService = filterEventsService;
    }

    private void OverlayOptions(SearchOptions userSearchOptions)
    {
        _mapper.Map(userSearchOptions, _searchOptions);
    }

    public async Task<IList<Event>> GetEvents(SearchOptions userSearchOptions)
    {
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

        events = _filterEventsService.FilterEvents(events, _searchOptions.AllPlaces, 
            _searchOptions.ExcludePlacesTerms!, 
            _searchOptions.InclidePlaces!, 
            _searchOptions.ExcludeEventsNamesTerms);

        events = _eventsGroupingService.EventsGrouping(events);

        return events;
    }
}
