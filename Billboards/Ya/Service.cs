using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Extension;
using Playbill.Billboards.Common.Service;
using Playbill.Common;
using Playbill.Common.Event;
using Playbill.Services.TitleNormalization.Common;

namespace Playbill.Billboards.Ya;

public class Service : ApiService<string>
{
    public Service(IOptions<Options> options, ITitleNormalization titleNormalizationService) : base(options, titleNormalizationService)
    {
    }

    public override BillboardTypes BillboardType => BillboardTypes.Ya;

    private string CreateRequest(DateOnly searchDate, string category, int days, int offset = 0)
    {
        var options = (_options as Options);

        var keys = new List<string>()
        {
            $"{options.QueryKeys?[QueryKeys.PageSize]}={options.QueryKeysConstants?[QueryKeys.PageSize]}",
            $"{options.QueryKeys?[QueryKeys.CurrentPage]}={offset}",
            $"{options.QueryKeys?[QueryKeys.DateFrom]}={searchDate.ToString(options.DateFormat)}",
            $"{options.QueryKeys?[QueryKeys.DateEnd]}={days}",
            $"{options.QueryKeys?[QueryKeys.Domain]}={options.QueryKeysConstants?[QueryKeys.Domain]}",
        };

        return options.BaseSearchUrl + category + "?" + string.Join('&', keys);
    }


    public override async Task<IList<Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, HashSet<EventTypes>? searchEventTypes = null)
    {
        var result = new List<Event>();

        var options = (_options as Options);

        var filteredEventKeys = EventKeys<string>(searchEventTypes);

        var pageSize = int.Parse(options.QueryKeysConstants?[QueryKeys.PageSize]);
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
                        var request = CreateRequest(searchDate, categoryId, days, offset);
                        var responseJson = await CallRequestAsync(request);
                        if (responseJson != null)
                        {
                            var respons = responseJson.ParseJson<Response>();
                            if (respons.Paging.Total >= offset + respons.Paging.Limit)
                            {
                                getData = true;
                            }
                            var events = respons.ConvertToEvents(convertToEventSetting);
                            result.AddRange(events);
                        }
                    } while (getData);
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
