using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Billboards.Common.Options;
using Playbill.Common;
using Playbill.Common.Event;
using Playbill.Services.LoadEvents.Common.Extension;

namespace Playbill.Services.LoadEvents.Common.Service;

public abstract class BaseLoadEventsService
{
    protected readonly IEnumerable<IBillboardService> _billboardService;
    protected readonly HashSet<EventTypes>? _searchEventTypes;
    protected readonly bool _removeUnidentifiedEventType;
    protected BaseLoadEventsService(IOptions<SearchOptions> searchOptions, 
        IEnumerable<IBillboardService> billboardService)
    {
        _billboardService = billboardService.FilterSupportedBillboards(searchOptions.Value?.SupportedBillboards);
        _searchEventTypes = searchOptions.Value?.SearchEventTypes;
        _removeUnidentifiedEventType = searchOptions.Value?.RemoveUnidentifiedEventType ?? false;
    }

    public abstract Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals);
}
