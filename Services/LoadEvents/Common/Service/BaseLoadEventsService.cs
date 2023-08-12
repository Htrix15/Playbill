using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Event;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Billboards.Common.Options;
using Playbill.Common;
using Playbill.Services.LoadEvents.Common.Extension;

namespace Playbill.Services.LoadEvents.Common.Service;

public abstract class BaseLoadEventsService
{
    protected readonly IEnumerable<IBillboardService> _billboardService;
    protected BaseLoadEventsService(IOptions<SupportedBillboardTypesOptions> billboards, 
        IEnumerable<IBillboardService> billboardService)
    {
        _billboardService = billboardService.FilterSupportedBillboards(billboards.Value?.SupportedTypes);
    }

    public abstract Task<IList<Event>> GetEvents(EventDateInterval eventDateInterval);
}
