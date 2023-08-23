using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Extension;
using Playbill.Billboards.Common.Options;
using Playbill.Common.Event;
using Playbill.Services.TitleNormalization.Common;

namespace Playbill.Billboards.Common.Service;

public abstract class PageParseService : BaseBillboardService
{
    protected PageParseService(IOptions<BaseOptions> options, ITitleNormalization titleNormalizationService) : base(options, titleNormalizationService)
    {
    }

    protected List<KeyValuePair<EventTypes, HashSet<string>>> EventKeys(HashSet<EventTypes>? searchEventTypes) =>
    (_options as PageParseOptions)?.EventKeys?.FilterEventKeys(searchEventTypes).ToList() ?? new List<KeyValuePair<EventTypes, HashSet<string>>>();
}
