using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Interfaces;
using Models.Billboards.Common.Logging;
using Models.Billboards.Common.Options;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.TitleNormalization.Common;


namespace Models.Billboards.Common.Service;

public abstract class BaseBillboardService(IOptions<BaseOptions> options, 
    ITitleNormalization titleNormalizationService,
    ILogger<BaseBillboardService> logger) : IBillboardService
{
    public abstract BillboardTypes BillboardType { get; }
    protected readonly BaseOptions _options = options.Value;
    protected readonly ITitleNormalization _titleNormalizationService = titleNormalizationService;

    public abstract Task<EventsResult> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes> searchEventTypes);

    public void StartGetEvents()
    {
        LogHelper.LogInformation(logger, BillboardType, BillboardLoadingState.Started);
    }

    public void EndGetEvents()
    {
        LogHelper.LogInformation(logger, BillboardType, BillboardLoadingState.End);
    }
}
