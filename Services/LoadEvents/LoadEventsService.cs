using Playbill.Billboards.Common.Interfaces;
using Playbill.Common;
using Playbill.Common.Event;
using Playbill.Services.LoadEvents.Common.Extension;
using Playbill.Services.LoadEvents.Common.Service;
using System.Collections.Concurrent;

namespace Playbill.Services.LoadEvents;

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
