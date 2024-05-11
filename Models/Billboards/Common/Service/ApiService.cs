using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Billboards.Common.Extension;
using Models.Billboards.Common.Logging;
using Models.Billboards.Common.Options;
using Models.Events;
using Models.ProcessingServices.TitleNormalization.Common;

namespace Models.Billboards.Common.Service;

public abstract class ApiService<T>(IOptions<BaseOptions> options, 
    ITitleNormalization titleNormalizationService,
    ILogger<ApiService<T>> logger) :
    BaseBillboardService(options, titleNormalizationService, logger)
{
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
                LogHelper.LogInformation(logger,
                    BillboardType,
                    BillboardLoadingState.Processing,
                    $"Fail call: {request} Remaining attempts: {attempt} Message : {exception.Message}");
                await Task.Delay(_options.TimeOut);
            }
        }
        while (attempt > 0);

        return null;
    }
}
