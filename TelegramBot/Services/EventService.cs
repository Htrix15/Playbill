using Microsoft.Extensions.Options;
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
    public EventService(IOptions<SearchOptions> defaultOptions, 
        MainService mainService, 
        IUserSettingsRepository userSettingsRepository)
    {
        _searchOptions = defaultOptions.Value;
        _mainService = mainService;
        _userSettingsRepository = userSettingsRepository;
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
        searchOptions.AddHolidays = userSettings.AddHolidays;
   
        searchOptions.DatePeriod = getEventsParams.DatePeriod;

        return await _mainService.GetEvents(searchOptions);
    }
}
