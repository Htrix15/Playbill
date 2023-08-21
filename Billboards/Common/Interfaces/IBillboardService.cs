using Playbill.Common;
using Playbill.Common.Event;

namespace Playbill.Billboards.Common.Interfaces;

public interface IBillboardService
{
    /// <param name="searchEventTypes">Use null for search all types</param>
    Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes>? searchEventTypes = null);
    public BillboardTypes BillboardType { get; }
}
