using Microsoft.Extensions.Options;
using Models.Billboards.Common.Extension;
using Models.Billboards.Common.Options;
using Models.Events;
using Models.ProcessingServices.TitleNormalization.Common;
using System.Net.Http.Headers;

namespace Models.Billboards.Common.Service;

public abstract class ApiService<T> : BaseBillboardService
{
    protected ApiService(IOptions<BaseOptions> options, ITitleNormalization titleNormalizationService) : base(options, titleNormalizationService)
    {
    }

    protected List<KeyValuePair<EventTypes, HashSet<TEventKey>>> EventKeys<TEventKey>(HashSet<EventTypes>? searchEventTypes) =>
        (_options as ApiOptions<TEventKey>)?.EventKeys?.FilterEventKeys(searchEventTypes).ToList() ?? new List<KeyValuePair<EventTypes, HashSet<TEventKey>>>();
   
    protected async Task<string?> CallRequestAsync(string request)
    {
        var httpClient = new HttpClient();
        var attempt = _options.TryCount;
        do
        {
            try
            {

                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, request);
                requestMessage.Headers.Add("x-force-cors-preflight", "1");
                using var response = await httpClient.SendAsync(requestMessage);
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
}
