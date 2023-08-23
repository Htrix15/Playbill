using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Extension;
using Playbill.Billboards.Common.Options;
using Playbill.Common.Event;
using Playbill.Services.TitleNormalization.Common;

namespace Playbill.Billboards.Common.Service;

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
}
