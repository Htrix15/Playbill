﻿using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Extension;
using Playbill.Billboards.Common.Options;

namespace Playbill.Billboards.Common.Service;

public abstract class PageParseService : BaseBillboardService
{
    protected PageParseService(IOptions<PageParseOptions> options) : base(options) { }

    protected List<KeyValuePair<EventTypes, HashSet<string>>> EventKeys(IList<EventTypes>? searchEventTypes) =>
    (_options as PageParseOptions)?.EventKeys?.FilterEventKeys(searchEventTypes).ToList() ?? new List<KeyValuePair<EventTypes, HashSet<string>>>();
}
