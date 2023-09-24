using Microsoft.EntityFrameworkCore;
using Models.Billboards;
using Models.Events;
using Models.Users;

namespace Repository;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly DbSet<UserSettings> _userSettings;
    private readonly ApplicationDbContext _applicationDbContext;

    public UserSettingsRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
        _userSettings = _applicationDbContext.UserSettings;
    }

    public async Task<UserSettings> GetUserSettingsAsync(long userId) => await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId);

    public async Task<bool> GetAddHolidaysAsync(long userId) => (await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId))?.AddHolidays ?? false;

    public async Task<List<BillboardTypes>> GetExcludeBillboardsAsync(long userId) => (await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId))?.ExcludeBillboards ?? new List<BillboardTypes>();

    public async Task<List<DayOfWeek>> GetExcludeDaysOfWeekAsync(long userId) => (await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId))?.ExcludeDaysOfWeek ?? new List<DayOfWeek>();

    public async Task<List<EventTypes>> GetExcludeEventTypesAsync(long userId) => (await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId))?.ExcludeEventTypes ?? new List<EventTypes>();

    public async Task<List<int>> GetExcludePlacesIdsAsync(long userId) => (await _userSettings
        .AsNoTracking()
        .FirstOrDefaultAsync(userSettings => userSettings.Id == userId))?.ExcludePlacesIds ?? new List<int>();

    public async Task UpdateAddHolidays(long userId, bool addHolidays)
    {
        if (await _userSettings.AnyAsync(userSettings => userSettings.Id == userId))
        {
            await _userSettings
                .Where(userSettings => userSettings.Id == userId)
                .ExecuteUpdateAsync(userSettings => userSettings
                .SetProperty(property => property.AddHolidays, addHolidays));
        } 
        else
        {
            await _userSettings.AddAsync(new UserSettings { Id = userId, AddHolidays = addHolidays });
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateExcludeBillboardsAsync(long userId, List<BillboardTypes> billboardTypes)
    {
        if (await _userSettings.AnyAsync(userSettings => userSettings.Id == userId))
        {
            await _userSettings
                .Where(userSettings => userSettings.Id == userId)
                .ExecuteUpdateAsync(userSettings => userSettings
                .SetProperty(property => property.ExcludeBillboards, billboardTypes));
        }
        else
        {
            await _userSettings.AddAsync(new UserSettings { Id = userId, ExcludeBillboards = billboardTypes });
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateExcludeDaysOfWeekAsync(long userId, List<DayOfWeek> dayOfWeeks)
    {
        if (await _userSettings.AnyAsync(userSettings => userSettings.Id == userId))
        {
            await _userSettings
                .Where(userSettings => userSettings.Id == userId)
                .ExecuteUpdateAsync(userSettings => userSettings
                .SetProperty(property => property.ExcludeDaysOfWeek, dayOfWeeks));
        }
        else
        {
            await _userSettings.AddAsync(new UserSettings { Id = userId, ExcludeDaysOfWeek = dayOfWeeks });
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateExcludeEventTypesAsync(long userId, List<EventTypes> eventTypes)
    {
        if (await _userSettings.AnyAsync(userSettings => userSettings.Id == userId))
        {
            await _userSettings
                .Where(userSettings => userSettings.Id == userId)
                .ExecuteUpdateAsync(userSettings => userSettings
                .SetProperty(property => property.ExcludeEventTypes, eventTypes));
        }
        else
        {
            await _userSettings.AddAsync(new UserSettings { Id = userId, ExcludeEventTypes = eventTypes });
        }

        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateExcludePlacesIds(long userId, List<int> placesIds)
    {
        if (await _userSettings.AnyAsync(userSettings => userSettings.Id == userId))
        {
            await _userSettings
                .Where(userSettings => userSettings.Id == userId)
                .ExecuteUpdateAsync(userSettings => userSettings
                .SetProperty(property => property.ExcludePlacesIds, placesIds));
        }
        else
        {
            await _userSettings.AddAsync(new UserSettings { Id = userId, ExcludePlacesIds = placesIds });
        }

        await _applicationDbContext.SaveChangesAsync();
    }
}
