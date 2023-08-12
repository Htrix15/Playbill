using Playbill.Common;

namespace Playbill.Billboards.Common.Interfaces;

public interface IBillboardService
{
    Task<IList<Event.Event>> GetEventsAsync(EventDateInterval eventDateInterval);
    public BillboardTypes BillboardType { get; }
}
