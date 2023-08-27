using Models.Events;
using Models.ProcessingServices.EventDateIntervals;

namespace Models.Billboards.Common.Interfaces;

public interface IBillboardService
{
    /// <param name="searchEventTypes">Use null for search all types</param>
    Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes>? searchEventTypes = null);
    public BillboardTypes BillboardType { get; }
}
