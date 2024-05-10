using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Enums;
using Models.Events;
using Models.Search;
using Models.Users;

namespace Repository;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly SearchOptions _searchOptions;
    private readonly DbSet<UserSettings> _userSettings;
    private readonly ApplicationDbContext _applicationDbContext;

    public UserSettingsRepository(IOptions<SearchOptions> defaultOptions, ApplicationDbContext applicationDbContext)
    {
        _searchOptions = defaultOptions.Value;
        _applicationDbContext = applicationDbContext;
        _userSettings = _applicationDbContext.UserSettings;
    }

    public async Task<UserSettings?> GetUserSettingsAsync(long userId) => await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId);

    public async Task<bool> GetAddHolidaysAsync(long userId) => (await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId))?.AddHolidays ?? false;

    public async Task<List<BillboardTypes>?> GetExcludeBillboardsAsync(long userId) => (await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId))?.ExcludeBillboards;

    public async Task<List<DayOfWeek>> GetExcludeDaysOfWeekAsync(long userId) => (await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId))?.ExcludeDaysOfWeek ?? new List<DayOfWeek>();

    public async Task<List<EventTypes>?> GetExcludeEventTypesAsync(long userId) => (await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId))?.ExcludeEventTypes;

    public async Task<List<int>> GetExcludePlacesIdsAsync(long userId) => (await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId))?.ExcludePlacesIds ?? new List<int>();

    public async Task UpdateAddHolidaysAsync(long userId, bool addHolidays)
    {
        if (await UserExsistAsync(userId))
        {
            await _userSettings
                .Where(userSettings => userSettings.Id == userId)
                .ExecuteUpdateAsync(userSettings => userSettings
                .SetProperty(property => property.AddHolidays, addHolidays));
        }
        else
        {
            var userSettings = new UserSettings(userId, _searchOptions)
            {
                AddHolidays = addHolidays
            };
            await _userSettings.AddAsync(userSettings);
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateExcludeBillboardsAsync(long userId, List<BillboardTypes> billboardTypes)
    {
        if (await UserExsistAsync(userId))
        {
            await _userSettings
                .Where(userSettings => userSettings.Id == userId)
                .ExecuteUpdateAsync(userSettings => userSettings
                .SetProperty(property => property.ExcludeBillboards, billboardTypes));
        }
        else
        {
            var userSettings = new UserSettings(userId, _searchOptions)
            {
                ExcludeBillboards = billboardTypes
            };
            await _userSettings.AddAsync(userSettings);
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateExcludeDaysOfWeekAsync(long userId, List<DayOfWeek> dayOfWeeks)
    {
        if (await UserExsistAsync(userId))
        {
            await _userSettings
                .Where(userSettings => userSettings.Id == userId)
                .ExecuteUpdateAsync(userSettings => userSettings
                .SetProperty(property => property.ExcludeDaysOfWeek, dayOfWeeks));
        }
        else
        {
            var userSettings = new UserSettings(userId, _searchOptions)
            {
                ExcludeDaysOfWeek = dayOfWeeks
            };
            await _userSettings.AddAsync(userSettings);
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateExcludeEventTypesAsync(long userId, List<EventTypes> eventTypes)
    {
        if (await UserExsistAsync(userId))
        {
            await _userSettings
                .Where(userSettings => userSettings.Id == userId)
                .ExecuteUpdateAsync(userSettings => userSettings
                .SetProperty(property => property.ExcludeEventTypes, eventTypes));
        }
        else
        {
            var userSettings = new UserSettings(userId, _searchOptions)
            {
                ExcludeEventTypes = eventTypes
            };
            await _userSettings.AddAsync(userSettings);
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateExcludePlacesIdsAsync(long userId, List<int> placesIds)
    {
        if (await UserExsistAsync(userId))
        {
            await _userSettings
                .Where(userSettings => userSettings.Id == userId)
                .ExecuteUpdateAsync(userSettings => userSettings
                .SetProperty(property => property.ExcludePlacesIds, placesIds));
        }
        else
        {
            var userSettings = new UserSettings(userId, _searchOptions)
            {
                ExcludePlacesIds = placesIds
            };
            await _userSettings.AddAsync(userSettings);
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task<bool> UserExsistAsync(long userId) => await _userSettings.AnyAsync(userSettings => userSettings.Id == userId);

}
