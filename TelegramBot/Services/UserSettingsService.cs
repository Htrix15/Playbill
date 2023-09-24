using Microsoft.Extensions.Options;
using Models.Billboards;
using Models.Events;
using Models.Places;
using Models.Search;
using Models.Users;
using TelegramBot.Params;
using static TelegramBot.Params.UserSettingsParams;

namespace TelegramBot.Services;

public class UserSettingsService
{
    private readonly SearchOptions _searchOptions;
    private readonly IPlaceRepository _placeRepository;
    private readonly IUserSettingsRepository _userSettingsRepository;
    public UserSettingsService(IOptions<SearchOptions> defaultOptions,
        IPlaceRepository placeRepository,
        IUserSettingsRepository userSettingsRepository)
    {
        _searchOptions = defaultOptions.Value;
        _placeRepository = placeRepository;
        _userSettingsRepository = userSettingsRepository;
    }

    private Func<T, Setting> ConverFunctions<T>() => billboard => new Setting()
    {
        Exclude = false,
        Id = Convert.ToInt32(billboard),
        Label = billboard.ToString()
    };


    private void SetExclude<T>(IEnumerable<T> excludeCollection, List<Setting> result)
    {
        excludeCollection.ToList().ForEach(excludeBillboard =>
        {
            var billboard = result.FirstOrDefault(billboard => billboard.Id == Convert.ToInt32(excludeBillboard));
            if (billboard is not null)
            {
                billboard.Exclude = true;
            }
        });
    }

    public async Task<List<Setting>> GetUserBillboardsSettingsAsync(long userId)
    {
        var result = _searchOptions.SupportedBillboards.Select(ConverFunctions<BillboardTypes>()).ToList();
        var userExcludes = await _userSettingsRepository.GetExcludeBillboardsAsync(userId);
        SetExclude(userExcludes, result);
        return result;
    }

    public async Task<List<Setting>> GetUserEventTypesSettingsAsync(long userId)
    {
        var result = _searchOptions.SearchEventTypes.Select(ConverFunctions<EventTypes>()).ToList();
        var userExcludes = await _userSettingsRepository.GetExcludeEventTypesAsync(userId);
        SetExclude(userExcludes, result);
        return result;
    }

    public async Task<List<Setting>> GetUserDaysOfWeekSettingsAsync(long userId)
    {
        var result = _searchOptions.DaysOfWeek.Select(ConverFunctions<DayOfWeek>()).ToList();
        var userExcludes = await _userSettingsRepository.GetExcludeDaysOfWeekAsync(userId);
        SetExclude(userExcludes, result);
        return result;
    }

    public async Task<List<Setting>> GetUserAddHolidaysSettingsAsync(long userId)
    {
        var result = new List<Setting>() {
            new Setting()
            {
                Exclude = ! await _userSettingsRepository.GetAddHolidaysAsync(userId),
                Id = 1,
                Label = "Искать в праздничные выходные"
            }
        };
        return result;
    }

    public async Task UpdateBillboardsSettingsAsync(UpdateUserSettingsParams settings)
    {
        var userId = settings.UserId;
        var excludes = await _userSettingsRepository.GetExcludeBillboardsAsync(userId);
        var key = Enum.Parse<BillboardTypes>(settings.EntityId);
        if (settings.Exclude)
        {
            excludes.Add(key);
        } else
        {
            excludes.Remove(key);
        }
        await _userSettingsRepository.UpdateExcludeBillboardsAsync(userId, excludes);
    }

    public async Task UpdateEventTypesSettingsAsync(UpdateUserSettingsParams settings)
    {
        var userId = settings.UserId;
        var excludes = await _userSettingsRepository.GetExcludeEventTypesAsync(userId);
        var key = Enum.Parse<EventTypes>(settings.EntityId);
        if (settings.Exclude)
        {
            excludes.Add(key);
        }
        else
        {
            excludes.Remove(key);
        }
        await _userSettingsRepository.UpdateExcludeEventTypesAsync(userId, excludes);
    }

    public async Task UpdateDaysOfWeekSettingsAsync(UpdateUserSettingsParams settings)
    {
        var userId = settings.UserId;
        var excludes = await _userSettingsRepository.GetExcludeDaysOfWeekAsync(userId);
        var key = Enum.Parse<DayOfWeek>(settings.EntityId);
        if (settings.Exclude)
        {
            excludes.Add(key);
        }
        else
        {
            excludes.Remove(key);
        }
        await _userSettingsRepository.UpdateExcludeDaysOfWeekAsync(userId, excludes);
    }

    public async Task UpdateAddHolidaysSettingsAsync(UpdateUserSettingsParams settings)
    {
        var userId = settings.UserId;
        await _userSettingsRepository.UpdateAddHolidays(userId, !settings.Exclude);
    }
}
