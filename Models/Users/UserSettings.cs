using Models.Billboards.Common.Enums;
using Models.Events;
using Models.Search;

namespace Models.Users;

public class UserSettings
{
    public long Id { get; set; } 
    public List<BillboardTypes>? ExcludeBillboards { get; set; }    
    public List<EventTypes>? ExcludeEventTypes { get; set; }
    public List<DayOfWeek>? ExcludeDaysOfWeek { get; set; }
    public List<int>? ExcludePlacesIds { get; set; }
    public bool AddHolidays { get; set; } = true;
    public UserSettings() { }
    public UserSettings (long userId, SearchOptions searchOptions) 
    {
        Id = userId;
        ExcludeBillboards = searchOptions?.ExcludeBillboards?.ToList() ?? new List<BillboardTypes>();
        ExcludeEventTypes = searchOptions?.ExcludeSearchEventTypes?.ToList() ?? new List<EventTypes>();
    }
}
