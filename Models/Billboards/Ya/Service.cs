using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Extension;
using Models.Billboards.Common.Logging;
using Models.Billboards.Common.Service;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.TitleNormalization.Common;

namespace Models.Billboards.Ya;

public class Service(IOptions<Options> options, 
    ITitleNormalization titleNormalizationService,
    ILogger<Service> logger) : ApiService<string>(options, titleNormalizationService, logger)
{
    public override BillboardTypes BillboardType => BillboardTypes.Ya;

    private string CreateRequest(DateOnly searchDate, string category, int days, int offset = 0)
    {
        var options = (_options as Options);

        var keys = new List<string>()
        {
            $"{options.QueryKeys?[QueryKeys.PageSize]}={options.QueryKeysConstants?[QueryKeys.PageSize]}",
            $"{options.QueryKeys?[QueryKeys.CurrentPage]}={offset}",
            $"{options.QueryKeys?[QueryKeys.DateFrom]}={searchDate.ToString(options.DateFormats.First())}",
            $"{options.QueryKeys?[QueryKeys.DateEnd]}={days}",
            $"{options.QueryKeys?[QueryKeys.Domain]}={options.QueryKeysConstants?[QueryKeys.Domain]}",
        };

        return options.BaseSearchUrl + category + "?" + string.Join('&', keys);
    }


    public override async Task<EventsResult> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes>? searchEventTypes = null)
    {
        var result = new List<Event>();

        var options = (_options as Options);

        var filteredEventKeys = EventKeys<string>(searchEventTypes);

        var pageSize = int.Parse(options.QueryKeysConstants?[QueryKeys.PageSize]);

        var stepCount = eventDateIntervals.Count * filteredEventKeys.Sum(f => f.Value.Count);
        var step = 1;
        LogHelper.LogInformation(logger,
                               BillboardType,
                               BillboardLoadingState.Processing,
                               stepCount: stepCount,
                               step: step);


        foreach (var eventKey in filteredEventKeys)
        {
            foreach (var eventDateInterval in eventDateIntervals)
            {
                foreach (var categoryId in eventKey.Value)
                {
                    var searchDate = eventDateInterval.StartDate;
                    var days = (eventDateInterval.EndDate.ToDateTime(TimeOnly.MaxValue).Date -
                        eventDateInterval.StartDate.ToDateTime(TimeOnly.MaxValue).Date).Days;

                    var offset = -pageSize;
                    bool getData;

                    var convertToEventSetting = new ConvertToEventSetting()
                    {
                        EventType = eventKey.Key,
                        BaseLinkUrl = options.BaseLinkUrl
                    };

                    do
                    {
                        getData = false;
                        offset += pageSize;
                        try
                        {
                            var request = CreateRequest(searchDate, categoryId, days, offset);
                            var responseJson = await CallRequestAsync(request);
                            if (responseJson != null)
                            {
                                try
                                {
                                    var respons = responseJson.ParseJson<Response>();
                                    if (respons.Paging.Total >= offset + respons.Paging.Limit)
                                    {
                                        getData = true;
                                    }
                                    var events = respons.ConvertToEvents(convertToEventSetting);
                                    result.AddRange(events);
                                }
                                catch (Exception exception)
                                {
                                    LogHelper.LogWarning(logger,
                                        BillboardType,
                                        BillboardLoadingState.Processing,
                                        $"Fail parse Json ({BillboardType}, {searchDate}, category: {categoryId}): {exception.Message}");
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            LogHelper.LogWarning(logger,
                                BillboardType,
                                BillboardLoadingState.Processing,
                                $"Fail call data ({BillboardType}, {searchDate}, category: {categoryId}): {exception.Message}");
                        }
                    }
                    while (getData);
                    
                    LogHelper.LogProgressInformation(logger,
                        BillboardType,
                        stepCount: stepCount,
                        step: step);

                    step++;
                }
            }
        }

        result = result.DateGrouping().ToList();

        result.ForEach(@event =>
        {
            @event.NormilizeTitle = _titleNormalizationService.TitleNormalization(@event.Title);
            @event.NormilizeTitleTerms = _titleNormalizationService.CreateTitleNormalizationTerms(@event.Title);
        });

        return new EventsResult()
        {
            Result = result,
        };
    }
}
