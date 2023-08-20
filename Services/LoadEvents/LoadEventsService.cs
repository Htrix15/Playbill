using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Billboards.Common.Options;
using Playbill.Common;
using Playbill.Common.Event;
using Playbill.Services.LoadEvents.Common.Service;
using System.Collections.Concurrent;

namespace Playbill.Services.LoadEvents;

public class LoadEventsService : BaseLoadEventsService
{
    public LoadEventsService(IOptions<SupportedBillboardTypesOptions> billboards, IEnumerable<IBillboardService> billboardService) : base(billboards, billboardService)
    {
    }

    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals)
    {
        var eventsBug = new ConcurrentBag<IList<Event>>();

        var loaders = _billboardService.Select(async service => eventsBug.Add(await service.GetEventsAsync(eventDateIntervals)));

        await Task.WhenAll(loaders.ToArray());

        var result = eventsBug.SelectMany(events => events).ToList();
        return result;
    }
}
