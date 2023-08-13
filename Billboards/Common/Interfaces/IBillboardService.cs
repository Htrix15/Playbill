using Playbill.Billboards.Common.Enums;
using Playbill.Common;

namespace Playbill.Billboards.Common.Interfaces;

public interface IBillboardService
{
    /// <param name="searchEventTypes">Use null for search all types</param>
    Task<IList<Event.Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, IList<EventTypes>? searchEventTypes = null);
    public BillboardTypes BillboardType { get; }
}
