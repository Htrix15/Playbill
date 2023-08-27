using Models.Billboards;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals.Common.Enums;

namespace Models.Search;

public class SearchOptions
{
    public HashSet<BillboardTypes>? SupportedBillboards { get; set; }
    public HashSet<EventTypes>? SearchEventTypes { get; set; }
    public HashSet<string>? ExcludePlacesTerms { get; set; }
    public bool? AllPlaces { get; set; }
    public Dictionary<EventTypes, HashSet<string>>? ExcludeEventsNamesTerms { get; set; }
    public HashSet<DayOfWeek>? DaysOfWeek { get; set; }
    public DatePeriods? DatePeriod { get; set; }
    /// <summary>
    /// Add holidays if date period contain their dates but DaysOfWeek exclude their day of week
    /// </summary>
    public bool? AddHolidays { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
