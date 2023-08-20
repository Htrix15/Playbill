using Playbill.Common;
using Playbill.Common.Event;

namespace Playbill.Billboards.Common.Interfaces;

public interface IBillboardService
{
    /// <param name="searchEventTypes">Use null for search all types</param>
    Task<IList<Playbill.Common.Event.Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, IList<EventTypes>? searchEventTypes = null);
    public BillboardTypes BillboardType { get; }
}
