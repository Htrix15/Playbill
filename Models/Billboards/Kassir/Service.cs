using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Extension;
using Models.Billboards.Common.Logging;
using Models.Billboards.Common.Service;
using Models.Events;
using Models.ProcessingServices.EventDateIntervals;
using Models.ProcessingServices.TitleNormalization.Common;

namespace Models.Billboards.Kassir;

public class Service(IOptions<Options> options, 
    ITitleNormalization titleNormalizationService,
    ILogger<Service> logger) : ApiService<int>(options, titleNormalizationService, logger)
{
    public override BillboardTypes BillboardType => BillboardTypes.Kassir;

    private string CreateRequest(DateOnly searchDate, int categoryId)
    {
        var options = (_options as Options);

        var keys = new List<string>()
        {
            $"{options.QueryKeys?[QueryKeys.CurrentPage]}={options.QueryKeysConstants?[QueryKeys.CurrentPage]}",
            $"{options.QueryKeys?[QueryKeys.PageSize]}={options.QueryKeysConstants?[QueryKeys.PageSize]}",
            $"{options.QueryKeys?[QueryKeys.Category]}={categoryId}",
            $"{options.QueryKeys?[QueryKeys.DateFrom]}={searchDate.ToString(options.DateFormat)}",
            $"{options.QueryKeys?[QueryKeys.DateEnd]}={searchDate.ToString(options.DateFormat)}",
            $"{options.QueryKeys?[QueryKeys.SortMode]}={options.QueryKeysConstants?[QueryKeys.SortMode]}",
            $"{options.QueryKeys?[QueryKeys.Domain]}={options.QueryKeysConstants?[QueryKeys.Domain]}",
        };

        return options.BaseSearchUrl + string.Join('&', keys);
    }

    public override async Task<EventsResult> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes>? searchEventTypes = null)
    {
        var result = new List<Event>();

        var options = (_options as Options);

        var eventTypes = options.EventKeys.Flip();
        var convertToEventSetting = new ConvertToEventSetting()
        {
            EventTypes = eventTypes,
            BasePathForLink = options.BaseLinkUrl,
            TimeOffset = options.TimeOffset
        };
        var filteredEventKeys = EventKeys<int>(searchEventTypes);

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
                    do
                    {
                        try
                        {
                            var request = CreateRequest(searchDate, categoryId);
                            var responseJson = await CallRequestAsync(request);
                            if (responseJson != null)
                            {
                                try
                                {
                                    var respons = responseJson.ParseJson<Response>();
                                    convertToEventSetting.EstimatedDate = searchDate;
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
                        searchDate = searchDate.AddDays(1);  
                    } while (searchDate <= eventDateInterval.EndDate);

                    LogHelper.LogProgressInformation(logger,
                        BillboardType,
                        stepCount: stepCount,
                        step: step,
                        progressStepLoging: 1);

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
