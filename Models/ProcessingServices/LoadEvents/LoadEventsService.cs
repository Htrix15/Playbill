using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Interfaces;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.LoadEvents.Common.Extension;
using Models.ProcessingServices.LoadEvents.Common.Service;
using System.Collections.Concurrent;

namespace Models.ProcessingServices.LoadEvents;

public class LoadEventsService : BaseLoadEventsService
{
    public LoadEventsService(IEnumerable<IBillboardService> billboardServices) : base(billboardServices)
    {
    }

    public override async Task<IList<Event>> GetEventsAsync(HashSet<BillboardTypes> supportedBillboards,
        IList<EventDateInterval> eventDateIntervals, 
        HashSet<EventTypes> searchEventTypes
        )
    {
        var eventsBug = new ConcurrentBag<IList<Event>>();

        var loaders = _billboardService
            .FilterSupportedBillboards(supportedBillboards)
            .Select(async service => eventsBug.Add(await service.GetEventsAsync(eventDateIntervals, searchEventTypes)));

        await Task.WhenAll(loaders.ToArray());

        var result = eventsBug.SelectMany(events => events).ToList();

        return result;
    }
}
