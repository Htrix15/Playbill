using Models.Billboards;
using Models.Events;

namespace Models.Users;

public interface IUserSettingsRepository
{
    Task<bool> UserExsistAsync(long userId);
    Task<UserSettings?> GetUserSettingsAsync(long userId);

    Task<List<BillboardTypes>?> GetExcludeBillboardsAsync(long userId);
    Task<List<EventTypes>?> GetExcludeEventTypesAsync(long userId);
    Task<List<DayOfWeek>> GetExcludeDaysOfWeekAsync(long userId);
    Task<List<int>> GetExcludePlacesIdsAsync(long userId);
    Task<bool> GetAddHolidaysAsync(long userId);

    Task UpdateExcludeBillboardsAsync(long userId, List<BillboardTypes> billboardTypes);
    Task UpdateExcludeEventTypesAsync(long userId, List<EventTypes> eventTypes);
    Task UpdateExcludeDaysOfWeekAsync(long userId, List<DayOfWeek> dayOfWeeks);
    Task UpdateExcludePlacesIds(long userId, List<int> placesIds);
    Task UpdateAddHolidays(long userId, bool addHolidays);
}
