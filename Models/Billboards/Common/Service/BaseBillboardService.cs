using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Interfaces;
using Models.Billboards.Common.Logging;
using Models.Billboards.Common.Options;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.TitleNormalization.Common;
using System.Globalization;


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

    protected DateTime ParseDate(string dateItemText, List<string> formats, bool tryAddYear = false)
    {
        foreach (var df in formats)
        {
            try
            {
                var date = DateTime.ParseExact(dateItemText, df, CultureInfo.CurrentCulture);
                if (DateTime.Now.Month > 10 && date.Month < 3)
                {
                    date = date.AddYears(1);
                }

                return date;
            }
            catch { }
        }

        if (tryAddYear)
        {
            return ParseDate($"{dateItemText},{DateTime.Now.AddYears(1).Year}", 
                formats.Select(f => $"{f},yyyy").ToList(), 
                false);
        }

        throw new Exception($"Fail parse date {dateItemText}");
    }
}
