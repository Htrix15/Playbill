using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Extension;
using Playbill.Billboards.Common.Service;
using Playbill.Common;
using Playbill.Common.Event;
using Playbill.Services.TitleNormalization.Common;

namespace Playbill.Billboards.Kassir;

public class Service : ApiService<int>
{
    public Service(IOptions<Options> options, ITitleNormalization titleNormalizationService) : base(options, titleNormalizationService)
    {
    }

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

    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes>? searchEventTypes = null)
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

        foreach (var eventKey in filteredEventKeys)
        {
            foreach (var eventDateInterval in eventDateIntervals)
            {
                foreach (var categoryId in eventKey.Value)
                {
                    var searchDate = eventDateInterval.StartDate;
                    do
                    {
                        var request = CreateRequest(searchDate, categoryId);
                        var responseJson = await CallRequestAsync(request);
                        if (responseJson != null)
                        {
                            var respons = responseJson.ParseJson<Response>();
                            convertToEventSetting.EstimatedDate = searchDate;
                            var events = respons.ConvertToEvents(convertToEventSetting);
                            result.AddRange(FilterEvents(events));
                        }
                        searchDate = searchDate.AddDays(1);
                    } while (searchDate <= eventDateInterval.EndDate);
                }
            }
        }

        result = result.DateGrouping().ToList();

        result.ForEach(@event =>
        {
            @event.NormilizeTitle = _titleNormalizationService.TitleNormalization(@event.Title);
            @event.NormilizeTitleTerms = _titleNormalizationService.CreateTitleNormalizationTerms(@event.Title);
        });

        return result;
    }
}
