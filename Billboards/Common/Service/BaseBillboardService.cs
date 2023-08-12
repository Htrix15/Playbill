using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Billboards.Common.Options;
using Playbill.Common;

namespace Playbill.Billboards.Common.Service;

public abstract class BaseBillboardService: IBillboardService
{
    public abstract BillboardTypes BillboardType { get; }
    protected readonly BaseOptions _options;

    protected BaseBillboardService(IOptions<BaseOptions> options)
    {
        _options = options.Value;
    }
    public abstract Task<IList<Event.Event>> GetEventsAsync(EventDateInterval eventDateInterval);
}
