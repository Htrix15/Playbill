using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Interfaces;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;

namespace Models.ProcessingServices.LoadEvents.Common.Service;

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
