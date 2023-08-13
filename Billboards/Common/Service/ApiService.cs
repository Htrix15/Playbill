using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Event;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Billboards.Common.Options;
using Playbill.Billboards.Kassir;
using System.Collections.Concurrent;

namespace Playbill.Billboards.Common.Service;

public abstract class ApiService<T> : BaseBillboardService
{
    protected ApiService(IOptions<ApiOptions<T>> options) : base(options) { }

    protected async Task<IList<string>> CallRequestsAsync(IList<string> requests)
    {
        var responsesBug = new ConcurrentBag<string>();

        var socketsHttpHandler = new SocketsHttpHandler()
        {
            MaxConnectionsPerServer = _options.MaxDegreeOfParallelism
        };
        var httpClient = new HttpClient(socketsHttpHandler);

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism };
        await Parallel.ForEachAsync(requests,
            parallelOptions,
            async (request, cancellationToken) =>
            {
                var attempt = _options.TryCount;
                do
                {
                    try
                    {
                        using var response = await httpClient.GetAsync(request, cancellationToken);
                        response.EnsureSuccessStatusCode();
                        responsesBug.Add(await response.Content.ReadAsStringAsync(cancellationToken));
                        break;
                    }
                    catch (Exception exception)
                    {
                        attempt--;
                        Console.WriteLine($"Fail call: {request} Remaining attempts: {attempt} Message : {exception.Message}");
                        await Task.Delay(_options.TimeOut, cancellationToken);
                    }
                }
                while (attempt > 0);
            }
        );

        return responsesBug.ToList();
    }

    protected (IList<Event.Event> events, IList<Event.Event> failedEvents) ConvertToEvents<TEventTypeKey>(List<IConvertToEvent<TEventTypeKey>> responses,
        Dictionary<TEventTypeKey, EventTypes> eventTypes, 
        string? basePathForLink = null,
        int timeOffset = 0)
    {
        var resultEvent = new List<Event.Event>();
        var resultFailedEvent = new List<Event.Event>();
        responses.ForEach(response =>
        {
            var events = response.ConvertToEvents(eventTypes, basePathForLink, timeOffset);
            resultEvent.AddRange(events.events);
            resultFailedEvent.AddRange(events.failedEvents);
        });

        return (resultEvent, resultFailedEvent);
    }
}
