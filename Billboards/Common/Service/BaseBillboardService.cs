using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Billboards.Common.Options;
using Playbill.Common;
using Playbill.Common.Event;
using Playbill.Services.TitleNormalization.Common;

namespace Playbill.Billboards.Common.Service;

public abstract class BaseBillboardService: IBillboardService
{
    public abstract BillboardTypes BillboardType { get; }
    protected readonly BaseOptions _options;
    protected readonly ITitleNormalization _titleNormalizationService;

    protected BaseBillboardService(IOptions<BaseOptions> options, ITitleNormalization titleNormalizationService)
    {
        _options = options.Value;
        _titleNormalizationService = titleNormalizationService;
    }

    public abstract Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes> searchEventTypes);
}
