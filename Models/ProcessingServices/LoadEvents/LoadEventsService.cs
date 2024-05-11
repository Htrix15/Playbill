using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Interfaces;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.LoadEvents.Common.Extension;
using Models.ProcessingServices.LoadEvents.Common.Service;
using System.Collections.Concurrent;

namespace Models.ProcessingServices.LoadEvents;

public class LoadEventsService(IEnumerable<IBillboardService> billboardServices) : BaseLoadEventsService(billboardServices)
{
    public override async Task<IList<EventsResult>> GetEventsAsync(HashSet<BillboardTypes> supportedBillboards,
        IList<EventDateInterval> eventDateIntervals, 
        HashSet<EventTypes> searchEventTypes
        )
    {
        var eventsBug = new ConcurrentBag<EventsResult>();

        var loaders = _billboardService
         .FilterSupportedBillboards(supportedBillboards)
         .Select(service => Task.Run(async () =>
         {
             service.StartGetEvents();
             eventsBug.Add(await service.GetEventsAsync(eventDateIntervals, searchEventTypes));
             service.EndGetEvents();
         }));

        await Task.WhenAll(loaders.ToArray());

        var result = eventsBug.ToList();

        return result;
    }
}
