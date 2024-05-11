using Models.Billboards.Common.Enums;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;

namespace Models.Billboards.Common.Interfaces;

public interface IBillboardService
{
    /// <param name="searchEventTypes">Use null for search all types</param>
    Task<EventsResult> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes>? searchEventTypes = null);
    public BillboardTypes BillboardType { get; }

    public void StartGetEvents();
    public void EndGetEvents();
}
