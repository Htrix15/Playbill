using Playbill.Billboards.Common.Enums;

namespace Playbill.Billboards.Common.Options;

public abstract class ApiOptions<TEventKeys> : BaseOptions
{
    public Dictionary<EventTypes, HashSet<TEventKeys>>? EventKeys { get; set; }
    public Dictionary<QueryKeys, string>? QueryKeys { get; set; }
}
