using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Extension;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Billboards.Common.Options;
using Playbill.Common.Event;
using System.Collections.Concurrent;

namespace Playbill.Billboards.Common.Service;

public abstract class ApiService<T> : BaseBillboardService
{
    protected ApiService(IOptions<ApiOptions<T>> options) : base(options) { }

    protected List<KeyValuePair<EventTypes, HashSet<TEventKey>>> EventKeys<TEventKey>(IList<EventTypes>? searchEventTypes) =>
        (_options as ApiOptions<TEventKey>)?.EventKeys?.FilterEventKeys(searchEventTypes).ToList() ?? new List<KeyValuePair<EventTypes, HashSet<TEventKey>>>();
   
    protected async Task<string?> CallRequestAsync(string request)
    {
        var httpClient = new HttpClient();
        var attempt = _options.TryCount;
        do
        {
            try
            {
                using var response = await httpClient.GetAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception exception)
            {
                attempt--;
                Console.WriteLine($"Fail call: {request} Remaining attempts: {attempt} Message : {exception.Message}");
                await Task.Delay(_options.TimeOut);
            }
        }
        while (attempt > 0);

        return null;
    }

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

    protected IList<Event> ConvertToEvents<TEventTypeKey>(List<IConvertToEvent<TEventTypeKey>> responses,
        BaseConvertToEventSetting convertToEventSetting)
    {
        var resultEvent = new List<Event>();
        responses.ForEach(response =>
        {
            var events = response.ConvertToEvents(convertToEventSetting);
            resultEvent.AddRange(events);
        });

        return resultEvent;
    }
}
