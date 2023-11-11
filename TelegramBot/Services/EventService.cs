using Microsoft.Extensions.Options;
using Models;
using Models.Places;
using Models.ProcessingServices;
using Models.Search;
using Models.Users;
using TelegramBot.Params;

namespace TelegramBot.Services;

public class EventService
{
    private readonly SearchOptions _searchOptions;
    private readonly MainService _mainService;
    private readonly IUserSettingsRepository _userSettingsRepository;
    private readonly IPlaceRepository _placeRepository;
    public EventService(IOptions<SearchOptions> defaultOptions, 
        MainService mainService,
        IRepository<UserSettings> userSettingsRepository,
        IRepository<Place> placeRepository)
    {
        _searchOptions = defaultOptions.Value;
        _mainService = mainService;
        _userSettingsRepository = (IUserSettingsRepository)userSettingsRepository;
        _placeRepository = (IPlaceRepository)placeRepository;
    }

    public async Task<IList<Models.Events.Event>> GetEvents(GetEventsParams getEventsParams)
    {
        var searchOptions = _searchOptions.Clone() as SearchOptions;
        var userSettings = await _userSettingsRepository.GetUserSettingsAsync(getEventsParams.UserId) ?? new UserSettings(0, _searchOptions);
    
        if (userSettings.ExcludeBillboards?.Any() ?? false)
        {
            searchOptions.SupportedBillboards = searchOptions.SupportedBillboards.Except(userSettings.ExcludeBillboards.ToHashSet()).ToHashSet();
        }
        if (userSettings.ExcludeDaysOfWeek?.Any() ?? false)
        {
            searchOptions.DaysOfWeek = searchOptions.DaysOfWeek.Except(userSettings.ExcludeDaysOfWeek.ToHashSet()).ToHashSet();
        }
        if (userSettings.ExcludeEventTypes?.Any() ?? false)
        {
            searchOptions.SearchEventTypes = searchOptions.SearchEventTypes.Except(userSettings.ExcludeEventTypes.ToHashSet()).ToHashSet();
        }
        if (userSettings.ExcludePlacesIds?.Any() ?? false)
        {
            searchOptions.ExcludePlacesTerms = (await _placeRepository.GetPlacesAsync(userSettings.ExcludePlacesIds, place => place.Name)).ToHashSet();
        }

        searchOptions.AddHolidays = userSettings.AddHolidays;
   
        searchOptions.DatePeriod = getEventsParams.DatePeriod;

        return await _mainService.GetEvents(searchOptions);
    }
}
