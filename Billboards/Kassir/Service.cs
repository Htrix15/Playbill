using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Event;
using Playbill.Billboards.Common.Extension;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Billboards.Common.Service;
using Playbill.Common;

namespace Playbill.Billboards.Kassir;

public class Service : ApiService<int>
{
    public Service(IOptions<Options> options) : base(options){}

    public override BillboardTypes BillboardType => BillboardTypes.Kassir;

    private IList<string> CreateRequest(IList<EventDateInterval> eventDateIntervals, IList<EventTypes>? searchEventTypes = null)
    {
        var options = (_options as Options);
        var requests = new List<string>();
        options?.EventKeys?.FilterEventKeys(searchEventTypes).ToList().ForEach(eventKey => {
            eventDateIntervals.ToList().ForEach(eventDateInterval =>
            {
                eventKey.Value.ToList().ForEach(categoryId =>
                {
                    var request = options.BaseUrl;
                    var keys = new List<string>()
                    {
                        $"{options.QueryKeys?[QueryKeys.СurrentPage]}={options.QueryKeysConstants?[QueryKeys.СurrentPage]}",
                        $"{options.QueryKeys?[QueryKeys.PageSize]}={options.QueryKeysConstants?[QueryKeys.PageSize]}",
                        $"{options.QueryKeys?[QueryKeys.Category]}={categoryId}",
                        $"{options.QueryKeys?[QueryKeys.DateFrom]}={eventDateInterval.StartDate.ToString(options.DateFormat)}",
                        $"{options.QueryKeys?[QueryKeys.DateEnd]}={eventDateInterval.EndDate.ToString(options.DateFormat)}",
                        $"{options.QueryKeys?[QueryKeys.SortMode]}={options.QueryKeysConstants?[QueryKeys.SortMode]}",
                        $"{options.QueryKeys?[QueryKeys.Domain]}={options.QueryKeysConstants?[QueryKeys.Domain]}",
                    };
                    request += string.Join('&', keys);
                    requests.Add(request);
                });
            });
        });
        return requests;
    }

    public override async Task<(IList<Event>, IList<Event>)> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, IList<EventTypes>? searchEventTypes = null)
    {
        var options = (_options as Options);
        var requests = CreateRequest(eventDateIntervals, searchEventTypes);
        var responsesJsons = await CallRequestsAsync(requests);
        var responses = responsesJsons.ParseJsons<Response>();
        var eventTypes = options.EventKeys.Flip();
        var convertToEventSetting = new ConvertToEventSetting()
        {
            EventTypes = eventTypes,
            BasePathForLink = options.QueryKeysConstants[QueryKeys.Domain],
            TimeOffset = options.TimeOffset
        };
        var events = ConvertToEvents(responses.ToList<IConvertToEvent<int>>(), convertToEventSetting);
        var validEvents = FilterEvents(events.events);
        var validFailedEvents = FilterEvents(events.failedEvents);
        return (validEvents, validFailedEvents);
    }


}
