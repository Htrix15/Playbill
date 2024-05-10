using Microsoft.Extensions.Options;
using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Interfaces;
using Models.Billboards.Common.Options;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.TitleNormalization.Common;

namespace Models.Billboards.Common.Service;

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
