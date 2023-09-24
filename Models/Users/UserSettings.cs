
using Models.Billboards;
using Models.Events;

namespace Models.Users;

public class UserSettings
{
    public long Id { get; set; } 
    public List<BillboardTypes>? ExcludeBillboards { get; set; }    
    public List<EventTypes>? ExcludeEventTypes { get; set; }
    public List<DayOfWeek>? ExcludeDaysOfWeek { get; set; }
    public List<int>? ExcludePlacesIds { get; set; }
    public bool AddHolidays { get; set; } = true;
}
