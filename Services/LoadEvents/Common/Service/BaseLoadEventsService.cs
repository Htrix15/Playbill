using Playbill.Billboards.Common.Interfaces;
using Playbill.Common;
using Playbill.Common.Event;

namespace Playbill.Services.LoadEvents.Common.Service;

public abstract class BaseLoadEventsService
{
    protected readonly IEnumerable<IBillboardService> _billboardService;
    protected BaseLoadEventsService(IEnumerable<IBillboardService> billboardService)
    {
        _billboardService = billboardService;
    }

    public abstract Task<IList<Event>> GetEventsAsync(HashSet<BillboardTypes> supportedBillboards, 
        IList<EventDateInterval> eventDateIntervals, 
        HashSet<EventTypes> searchEventTypes);
}
